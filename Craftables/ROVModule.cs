using UnityEngine;
using xale.Subnautica.PressureVessel.Behaviours;

namespace xale.Subnautica.PressureVessel.Craftables;

internal static class ROVModule
{
    public static TechType rovModule { get; private set; }

    public static void Register()
    {
        // First, register the ROV itself.
        ROV.RegisterPrefab();

        rovModule =
            VehicleModuleBuilder
                .WithTechType(
                    "RovModule",
                    "Remora Drone Module",
                    "Deployable remotely-operated mini-submersible. Prawn/Seamoth compatible.")
                .SetIcon(SpriteManager.Get(TechType.MapRoomCamera)) // TODO(xale): custom sprite
                .SetQuickSlotType(QuickSlotType.Selectable)
                .SetRecipeIngredients(
                    new CraftData.Ingredient(TechType.ComputerChip),
                    new CraftData.Ingredient(TechType.Battery),
                    new CraftData.Ingredient(TechType.WiringKit),
                    new CraftData.Ingredient(TechType.Glass),
                    new CraftData.Ingredient(TechType.Titanium, 3))
                .SetLinkedItems(ROV.rov)
                .SetFabricatorPath("CommonModules")
                .SetCraftingTimeSeconds(7f)
                .SetOnModuleAdded(OnAdded)
                .SetOnModuleRemoved(OnRemoved)
                .SetOnModuleUsed(OnUsed)
                .RegisterAndGetTechType();

        // TODO(xale): configure prerequisites
    }

    internal static void OnAdded(Vehicle vehicle, int _slotId)
    {
        ConfigureROVDocking(vehicle);

        // TODO(xale): remove drone from player's inventory, if present, and dock
    }

    private static void ConfigureROVDocking(Vehicle vehicle)
    {
        // Create a region attached to the vehicle for the drone to dock.
        SphereCollider dockingPort = vehicle.gameObject.AddComponent<SphereCollider>();
        // TODO(xale): determine location based on vehicle
        dockingPort.center = new Vector3(0, -1.5f, 0);
        dockingPort.radius = 1;
        dockingPort.isTrigger = true;

        // TODO(xale): add model for docking port

        // Create an "empty" point in front of the vehicle for releasing the drone.
        GameObject releasePoint = new GameObject();
        releasePoint.transform.parent = vehicle.gameObject.transform;
        releasePoint.transform.localPosition = new Vector3(0, 0, 2);
        releasePoint.transform.localRotation = Quaternion.identity;

        ROVDocking dockingSystem = vehicle.gameObject.AddComponent<ROVDocking>();
        dockingSystem.dockingPort = dockingPort;
        dockingSystem.dockingTransform = dockingPort.transform;
        dockingSystem.releasePoint = releasePoint;
        dockingSystem.undockTransform = releasePoint.transform;
    }

    internal static void OnRemoved(Vehicle vehicle, int _slotId)
    {
        // TODO(xale): (beforehand:) check space in player's inventory
        // TODO(xale): move drone, if docked, to player's inventory

        RemoveROVDocking(vehicle);
    }

    private static void RemoveROVDocking(Vehicle vehicle)
    {
        ROVDocking dockingPort = vehicle.gameObject.GetComponent<ROVDocking>();
        if (dockingPort == null) { return; }

        GameObject.Destroy(dockingPort.releasePoint);
        GameObject.Destroy(dockingPort.dockingPort);
        GameObject.Destroy(dockingPort);
    }

    internal static void OnUsed(Vehicle vehicle, int _slotId, float _charge, float _chargeScalar)
    {
        // If the drone is currently docked to the vehicle, release it.
        ROVDocking dockingSystem = vehicle.gameObject.GetComponent<ROVDocking>();
        if (dockingSystem != null && dockingSystem.camera != null)
        {
            // TODO(xale): check release point is clear
            dockingSystem.UndockROV();
        }

        // Check whether an ROV is available and controllable.
        ROV rov = dockingSystem.lastDocked;
        if (rov == null)
        {
            // TODO(xale): check for an ROV nearby
            ErrorMessage.AddError("No Remoras available.");
            return;
        }

        if (!rov.CanBeControlled())
        {
            ErrorMessage.AddError(
                "Remora not responding - check for damage or low battery.");
            return;
        }

        rov.Control(/* mothership= */ vehicle);
    }
}
