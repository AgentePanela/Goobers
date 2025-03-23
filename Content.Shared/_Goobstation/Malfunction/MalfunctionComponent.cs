using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Content.Shared.StatusIcon;

namespace Content.Shared._Goobstation.Malfunction;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class MalfunctionComponent : Component
{

    public List<ProtoId<EntityPrototype>> BaseMalfunctionActions = new()
    {
        "ModuleMenu"/*,
        "Cyborg Hijack"*/
    };
    [DataField, AutoNetworkedField] public float ControlPower = 5f;
    [DataField, AutoNetworkedField] public float MaxControlPower = 550f;
}
