using Content.Goobstation.Shared.Malfunction;
using Content.Shared.Store.Components;

namespace Content.Goobstation.Server.Malfunction;

public sealed partial class MalfunctionSystem : EntitySystem
{
    //[Dependency] private readonly ExplosionSystem _explosionSystem = default!;

    public void SubscribeAbilities()
    {
        SubscribeLocalEvent<MalfunctionComponent, OpenModuleMenuEvent>(OnStore);
    }
    private void OnStore(EntityUid uid, MalfunctionComponent comp, ref OpenModuleMenuEvent args)
    {
        Log.Debug(uid.ToString());
        if (!TryComp<StoreComponent>(uid, out var store))
            return;
        Log.Debug("Well...");

        _store.ToggleUi(uid, uid, store);
    }
}
