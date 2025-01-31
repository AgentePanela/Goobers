using Content.Server.Antag;
using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Server.Roles;
using Content.Shared.Roles;
using Content.Shared.Store;
using Content.Shared.Store.Components;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using System.Text;
using Content.Server.GameTicking.Rules;
using Content.Server._Gabystation.GameTicking.Rules.Components;
using Content.Shared._Gabystation.MalfAi;
using Content.Shared.Silicons.StationAi;

namespace Content.Server._Gabystation.GameTicking.Rules;

public sealed partial class MalfAiRuleSystem : GameRuleSystem<MalfAiRuleComponent>
{
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;
    [Dependency] private readonly SharedStationAiSystem _stationAi = default!;
    public readonly ProtoId<AntagPrototype> MalfAiPrototypeId = "MalfAi";
    public readonly ProtoId<CurrencyPrototype> Currency = "ControlPower";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MalfAiRuleComponent, AfterAntagEntitySelectedEvent>(AfterAntagSelected);
    }
    private void AfterAntagSelected(EntityUid uid, MalfAiRuleComponent comp, ref AfterAntagEntitySelectedEvent args)
    {
        Log.Debug("Geting MalfAi result...");
        if (MakeMalfAi(args, comp))
            Log.Debug("MalfAi result: True");
        else
            Log.Debug("MalfAi result: False");
    }
    public bool MakeMalfAi(AfterAntagEntitySelectedEvent args, MalfAiRuleComponent rule)
    {
        EntityUid target = args.EntityUid;

        if (!_mind.TryGetMind(args.EntityUid, out var mindId, out var mind))
        {
            Log.Debug("Get mind");
            return false;
        }
        /*
        if (!TryComp<StationAiCoreComponent>(mindId, out var aiCore))
        {
            Log.Debug("Station Ai");
            Log.Debug($"MindId: {mindId.ToString()} - EUid: {args.EntityUid.ToString()}");
            return false;
        }
        if (!_stationAi.TryGetInsertedAI((args.EntityUid, aiCore), out var aiTarget))
        {
            Log.Debug("Insert Ai");
            return false;
        }

        target = aiTarget.Value;*/

        EnsureComp<MalfAiComponent>(target);


        var store = EnsureComp<StoreComponent>(target);
        foreach (var category in rule.StoreCategories)
            store.Categories.Add(category);
        store.CurrencyWhitelist.Add(Currency);
        store.Balance.Add(Currency, 16);

        rule.MalfAiMind.Add(mindId);

        foreach (var objective in rule.Objectives)
            _mind.TryAddObjective(mindId, mind, objective);

        Log.Debug(target.ToString());

        return true;
    }

}
