using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Goobstation.MalfAi;

public sealed partial class MalfAiActionComponent : Component
{
    [DataField] public float ControlPowerCost = 0;
}

public sealed partial class OpenModuleMenuEvent : InstantActionEvent { }
