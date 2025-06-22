using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Malfunction;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class MalfunctionComponent : Component
{

    [DataField]
    public List<ProtoId<EntityPrototype>> BaseMalfunctionActions = new()
    {
        "ModuleMenu"/*,
        "Cyborg Hijack"*/
    };
    [DataField, AutoNetworkedField] public float ControlPower = 5f;
    [DataField, AutoNetworkedField] public float MaxControlPower = 550f;
}
