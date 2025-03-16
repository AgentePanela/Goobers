using Content.Shared.Roles;
using Content.Shared.Store;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Content.Server.GameTicking;
using Content.Server._Goobstation.GameTicking.Rules;

namespace Content.Server._Goobstation.GameTicking.Rules.Components;

/*
    Não tenho ideia nenhuma do que to fazendo
*/

[RegisterComponent, Access(typeof(MalfAiRuleSystem))]
public sealed partial class MalfAiRuleComponent : Component
{
    public readonly List<EntityUid> MalfAiMind = new();

    public readonly List<ProtoId<StoreCategoryPrototype>> StoreCategories = new()
    {
        "MalfAiUtilityModules",
        /*"MalfAiDestructiveModules",
        "MalfAiUpgradeModules"*/
    };
    public readonly List<ProtoId<EntityPrototype>> Objectives = new()
    {
        "MalfAiSurviveObjective"
    };
}
