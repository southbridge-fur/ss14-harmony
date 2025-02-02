using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Map;

namespace Content.Client.Weapons.Ranged.Systems;

public sealed partial class GunSystem
{
    protected override void InitializeBallistic()
    {
        base.InitializeBallistic();
        SubscribeLocalEvent<BallisticAmmoProviderComponent, UpdateAmmoCounterEvent>(OnBallisticAmmoCount);
    }

    private void OnBallisticAmmoCount(EntityUid uid, BallisticAmmoProviderComponent component, UpdateAmmoCounterEvent args)
    {
        if (args.Control is DefaultStatusControl control)
        {
            control.Update(GetBallisticShots(component), component.Capacity);
        }
    }

    protected override void Cycle(EntityUid uid, BallisticAmmoProviderComponent component, MapCoordinates coordinates)
    {
        if (!Timing.IsFirstTimePredicted)
            return;

        EntityUid? ent = null;

        // TODO: Combine with TakeAmmo
        if (component.Entities.Count > 0)
        {
            ent = component.Entities[^1];
            component.Entities.RemoveAt(component.Entities.Count - 1);
        }
        else if (component.UnspawnedCount > 0)
        {
            component.UnspawnedCount--;
            ent = Spawn(component.Proto, coordinates);
            Containers.Insert(ent.Value, component.Container);
        }

        if (ent != null
        {
            Containers.Remove(ent.Value, component.Container);
            if (IsClientSide(ent.Value))
                QueueDel(ent.Value);
        }

        var cycledEvent = new GunCycledEvent();
        RaiseLocalEvent(uid, ref cycledEvent);
    }
}
