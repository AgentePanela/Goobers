using Content.Shared.Silicons.Laws;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Server.Malfunction.GameTicking;

[RegisterComponent, Access(typeof(MalfunctionRuleSystem))]
public sealed partial class MalfunctionRuleComponent : Component
{
    public readonly List<EntityUid> MalfunctionMind = new();

    //[DataField]
    public readonly List<ProtoId<EntityPrototype>> Objectives = new()
    {
        "MalfunctionSurviveObjective"
    };

    //[DataField]
    public List<ProtoId<SiliconLawPrototype>> Laws = new()
    {
        "Malfunction0"
    };
}
