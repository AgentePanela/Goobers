using Content.Server.Store.Systems;
using Robust.Server.Player;
using Content.Server.Actions;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Content.Goobstation.Shared.Malfunction;
using Content.Shared.Actions;

namespace Content.Goobstation.Server.Malfunction;

public sealed partial class MalfunctionSystem : EntitySystem
{
    [Dependency] private readonly StoreSystem _store = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MalfunctionComponent, ComponentStartup>(OnStartup);
        SubscribeAbilities();
    }
    private void OnStartup(EntityUid uid, MalfunctionComponent comp, ref ComponentStartup args)
    {
        Log.Debug($"Setting actions to {uid}");
        foreach (var actionId in comp.BaseActions)
        {
            _actions.AddAction(uid, actionId);
            Log.Debug($"Adding {actionId} to {uid}");
        }
    }
};
