using Content.Server.Explosion.EntitySystems;
using Content.Server.Chat.Managers;
using Content.Server.Power.Components;
using Content.Server.Objectives.Components;
using Content.Server.Store.Systems;
using Content.Shared.Emag.Components;
using Content.Shared._Gabystation.MalfAi;
using Content.Shared.FixedPoint;
using Content.Shared.Actions;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Mobs;
using Content.Shared.Store.Components;
using Content.Shared.Popups;
using Content.Shared.RCD.Components;
using Robust.Shared.Prototypes;
using Robust.Server.GameObjects;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Server._Gabystation.MalfAi;

public sealed partial class MalfAiSystem : EntitySystem
{
    //[Dependency] private readonly ExplosionSystem _explosionSystem = default!;

    public void SubscribeAbilities()
    {
        SubscribeLocalEvent<MalfAiComponent, OpenModuleMenuEvent>(OnOpenModuleMenu);
    }
    private void OnOpenModuleMenu(EntityUid uid, MalfAiComponent comp, ref OpenModuleMenuEvent args)
    {
        Log.Debug(uid.ToString());
        if (!TryComp<StoreComponent>(uid, out var store))
            return;

        _store.ToggleUi(uid, uid, store);
    }
}
