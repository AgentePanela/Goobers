using Robust.Shared.Audio;

namespace Content.Server.Silicons.StationAi;

[RegisterComponent]
public sealed partial class StationAiRequirePowerComponent : Component
{
    [ViewVariables]
    public bool IsPowered = true;

    [DataField]
    public float Wattage = 10f;

    [DataField]
    public bool ApcOffWarned = false;

    [DataField]
    public int LastAnnouncedPower = 100;

    [DataField]
    public SoundSpecifier? WarningSound = new SoundPathSpecifier("/Audio/Misc/notice2.ogg");
    [DataField]
    public SoundSpecifier? DeathSound = new SoundPathSpecifier("/Audio/Misc/notice2.ogg");
}
