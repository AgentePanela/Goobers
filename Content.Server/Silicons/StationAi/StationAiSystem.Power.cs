using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Actions;
using Content.Shared.Chat;
using Content.Shared.Movement.Components;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Silicons.StationAi;
using Content.Shared.StationAi;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Silicons.StationAi;

public sealed partial class StationAiSystem
{
    [Dependency] private readonly BatterySystem _battery = default!;
    [Dependency] private readonly SharedActionsSystem _action = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    [ValidatePrototypeId<EntityPrototype>]
    private readonly string _deathActionId = "ActionCritSuccumb";
    private void InitializePower()
    {
        SubscribeLocalEvent<StationAiRequirePowerComponent, PowerChangedEvent>(OnCorePowerChange);
        SubscribeLocalEvent<StationAiRequirePowerComponent, IntellicardAttemptEvent>(OnIntellicardAttempt);
        SubscribeLocalEvent<StationAiRequirePowerComponent, ChargeChangedEvent>(OnCoreBatteryChange);
    }

    private void UpdatePower(float frameTime)
    {
        var query = EntityQueryEnumerator<StationAiRequirePowerComponent, StationAiCoreComponent, BatteryComponent>();
        while (query.MoveNext(out var uid, out var aiPower, out var core, out var battery))
        {
            // Do not use battery power when theres no player in this core
            if (!TryGetHeld((uid, core), out _))
            {
                _battery.SetCharge(uid, battery.MaxCharge, battery);
                continue;
            }

            if (aiPower.IsPowered)
            {
                if (!TryComp<ApcPowerReceiverComponent>(uid, out var power))
                    continue;

                if (_battery.IsFull(uid, battery))
                    continue;

                var inputCharge = power.Load > power.PowerReceived ? power.PowerReceived : power.Load;
                var newCharge = battery.CurrentCharge + inputCharge * frameTime;
                _battery.SetCharge(uid, newCharge, battery);
            }
            else
            {
                if (!_battery.TryUseCharge(uid, aiPower.Wattage * frameTime, battery))
                {
                    aiPower.IsPowered = false;
                    TurnOff(uid, core);
                    _battery.SetCharge(uid, 0f, battery);
                }
            }
        }
    }

    private void OnCorePowerChange(EntityUid uid, StationAiRequirePowerComponent component, ref PowerChangedEvent args)
    {
        UpdatePoweredState(uid, component);
    }
    private void OnCoreBatteryChange(EntityUid uid, StationAiRequirePowerComponent comp, ref ChargeChangedEvent args)
    {
        var percent = (int) ((args.Charge / args.MaxCharge) * 100);
        var clamp = Math.Clamp((percent / 25) * 25, 0, 100);

        if (clamp < comp.LastAnnouncedPower && clamp > 0)
        {
            var msg = Loc.GetString("ai-power-warning-backup", ("power", clamp));
            AnnounceAi(uid, msg, comp.WarningSound);
            comp.LastAnnouncedPower = clamp;
        }
    }

    private void OnIntellicardAttempt(EntityUid uid, StationAiRequirePowerComponent component, IntellicardAttemptEvent args)
    {
        if (component.IsPowered || args.Cancelled)
            return;

        args.Cancel();
        _popup.PopupEntity(Loc.GetString("base-computer-ui-component-not-powered", ("machine", uid)), args.User, args.User, PopupType.MediumCaution);
    }

    private void UpdatePoweredState(EntityUid uid, StationAiRequirePowerComponent component)
    {
        if (!TryComp<ApcPowerReceiverComponent>(uid, out var receiver))
            return;

        if (receiver.Powered == component.IsPowered)
            return;

        if (!TryComp<StationAiCoreComponent>(uid, out var core))
            return;

        component.IsPowered = receiver.Powered;
        component.ApcOffWarned = !receiver.Powered;

        if (receiver.Powered)
        {
            TurnOn(uid, core);
        }
        else
        {
            if (component.ApcOffWarned)
                return;

            if (!TryGetHeld((uid, core), out var ai))
                return;

            var msg = Loc.GetString("ai-power-warning");
            AnnounceAi(ai, msg, component.WarningSound);

            // dont run turn off now as it will instantly kill the ai
        }
    }

    private void TurnOff(EntityUid uid, StationAiCoreComponent core)
    {
        // TODO: state
        //* What happen when theres no holder and the AI dies? (like, job slot)

        if (TryGetHeld((uid, core), out var ai))
            return;

        // TODO: Radio

        // Do the death message
        TryComp<StationAiRequirePowerComponent>(uid, out var comp);
        var msg = Loc.GetString("ai-power-death-message");
        AnnounceAi(ai, msg, comp?.DeathSound);

        // Remove vision
        if (TryComp<StationAiVisionComponent>(core.RemoteEntity, out var vision))
        {
            vision.Range = 1.5f; // Like crit state
        }
        // Disable eye moving
        if (TryComp<InputMoverComponent>(ai, out var mover))
        {
            mover.CanMove = false;
            if (core.RemoteEntity is not null) // TP holder to core pos
                _xforms.DropNextTo(core.RemoteEntity.Value, uid);
        }
        // Add dead avatar //todo: pass this to dead state in shared side
        //_appearance.SetData(ai, StationAiVisualState.Key, StationAiState.Dead);

        // Remove all actions and add the become a ghost action
        RemComp<ActionGrantComponent>(ai);
        _action.AddAction(uid, _deathActionId);

        //EntityManager.DeleteEntity(ai);
        //ClearEye((uid, core));
    }

    private void TurnOn(EntityUid uid, StationAiCoreComponent core)
    {
        if (!SetupEye((uid, core)))
            return;

        AttachEye((uid, core));
    }
}
