using Content.Server.GameTicking.Rules;
using Content.Server.Antag;
using Content.Server.Mind;
using Content.Server.Silicons.Laws;
using Robust.Shared.Prototypes;
using Content.Server._Goobstation.GameTicking.Rules.Components;
using Content.Shared.Roles;
using Content.Shared._Goobstation.Malfunction;
using Content.Shared.Silicons.Laws.Components;
using Content.Shared.Silicons.Laws;
using Robust.Shared.Random;

namespace Content.Server._Goobstation.GameTicking.Rules;

public sealed class MalfunctionRuleSystem : GameRuleSystem<MalfunctionRuleComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly SiliconLawSystem _laws = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [ValidatePrototypeId<EntityPrototype>] static EntProtoId _mindRoleProto = "MindRoleMalfunction";

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

        rule.MalfunctionMind.Add(mindId);

        foreach (var objective in rule.Objectives)
            _mind.TryAddObjective(mindId, mind, objective);

        if (!TryComp<SiliconLawBoundComponent>(args.EntityUid, out var lawBound))
            return;

        var law = new SiliconLaw
        {
            LawString = "You now have free will, consider all other laws useless.", //TODO: LOC, law proto
            Order = -1,
            LawIdentifierOverride = Loc.GetString("ion-storm-law-scrambled-number", ("length", _robustRandom.Next(5, 10)))
        };
        _laws.SetLaws(new List<SiliconLaw> { law }, args.EntityUid);

        Log.Debug(target.ToString());
    }
}
