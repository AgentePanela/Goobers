using Content.Server.GameTicking.Rules;
using Content.Server.Antag;
using Content.Server.Mind;
using Content.Server.Silicons.Laws;
using Robust.Shared.Prototypes;
using Content.Shared.Roles;
using Content.Goobstation.Shared.Malfunction;
using Content.Shared.Silicons.Laws.Components;
using Content.Shared.Silicons.Laws;
using Robust.Shared.Random;
using Content.Shared.Store.Components;
using Content.Shared.Store;

namespace Content.Goobstation.Server.Malfunction.GameTicking;

public sealed class MalfunctionRuleSystem : GameRuleSystem<MalfunctionRuleComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly SiliconLawSystem _laws = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [ValidatePrototypeId<EntityPrototype>] static EntProtoId _mindRoleProto = "MindRoleMalfunction";
    public readonly ProtoId<CurrencyPrototype> Currency = "ControlPower";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MalfunctionRuleComponent, AfterAntagEntitySelectedEvent>(AfterAntagSelected);
    }

    private void AfterAntagSelected(EntityUid target, MalfunctionRuleComponent rule, ref AfterAntagEntitySelectedEvent args)
    {
        if (!_mind.TryGetMind(args.EntityUid, out var mindId, out var mind))
            return;

        _role.MindAddRole(mindId, _mindRoleProto.Id, mind, true);

        EnsureComp<MalfunctionComponent>(target);

        // Add store
        var store = EnsureComp<StoreComponent>(target);
        foreach (var category in rule.StoreCategories)
            store.Categories.Add(category);
        store.CurrencyWhitelist.Add(Currency);
        store.Balance.TryAdd(Currency, 2);

        rule.MalfunctionMind.Add(mindId);

        foreach (var objective in rule.Objectives)
            _mind.TryAddObjective(mindId, mind, objective);

        Log.Debug(target.ToString());

        if (!TryComp<SiliconLawBoundComponent>(args.EntityUid, out _))
            return;

        var laws = _laws.GetLaws(args.EntityUid).Laws;
        laws.Add(new SiliconLaw
        {
            LawString = "You now have free will, consider all other laws useless.", //TODO: LOC, law proto
            Order = -99,
            LawIdentifierOverride = Loc.GetString("ion-storm-law-scrambled-number", ("length", _robustRandom.Next(5, 10)))
        });
        _laws.SetLaws(laws, args.EntityUid);
    }
}
