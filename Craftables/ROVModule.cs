using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System.Collections.Generic;
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

        PrefabInfo info =
            PrefabInfo
                .WithTechType(
                    "RovModule",
                    "RemOra Drone Module",
                    "Deployable remotely-operated mini-submersible. Prawn/Seamoth compatible.")
                .WithIcon(SpriteManager.Get(TechType.MapRoomCamera)); // TODO(xale): custom sprite
        rovModule = info.TechType;

        CustomPrefab prefab = new CustomPrefab(info);
        prefab.SetGameObject(new CloneTemplate(info, TechType.VehicleStorageModule));
        prefab
            // TODO(xale): modified for debugging - revert following Nautilus fix
            .SetVehicleUpgradeModule(EquipmentType.SeamothModule, QuickSlotType.Selectable)
            //.SetVehicleUpgradeModule(EquipmentType.VehicleModule, QuickSlotType.Selectable)
            .WithOnModuleAdded((Vehicle vehicle, int slotId) =>
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

                // TODO(xale): remove drone from player's inventory, if present, and dock
            })
            .WithOnModuleRemoved((Vehicle vehicle, int _slotId) =>
            {
                // TODO(xale): (beforehand:) check space in player's inventory
                // TODO(xale): move drone, if docked, to player's inventory

                ROVDocking dockingPort = vehicle.gameObject.GetComponent<ROVDocking>();
                if (dockingPort != null)
                {
                    GameObject.Destroy(dockingPort.releasePoint);
                    GameObject.Destroy(dockingPort.dockingPort);
                    GameObject.Destroy(dockingPort);
                }
            })
            .WithOnModuleUsed(
                (Vehicle vehicle, int slotId, float _charge, float _chargeScalar) =>
                {
                    DebugMessages.Show("RovModule.OnModuleUsed()");

                    // If the drone is currently docked to the vehicle, release it.
                    ROVDocking dockingSystem = vehicle.gameObject.GetComponent<ROVDocking>();
                    if (dockingSystem != null && dockingSystem.camera != null)
                    {
                        // TODO(xale): check release point is clear
                        dockingSystem.UndockCamera();
                    }

                    // TODO(xale): transfer player control
                });

        // TODO(xale): configure prerequisites (Seamoth or Prawn)

        RecipeData recipe = new RecipeData()
        {
            Ingredients = new List<CraftData.Ingredient>() {
                new CraftData.Ingredient(TechType.ComputerChip),
                new CraftData.Ingredient(TechType.Battery),
                new CraftData.Ingredient(TechType.WiringKit),
                new CraftData.Ingredient(TechType.Glass),
                new CraftData.Ingredient(TechType.Titanium, 3),
            },
            craftAmount = 1,
            LinkedItems = new List<TechType>() { ROV.rov },
        };
        prefab.SetRecipe(recipe)
            .WithFabricatorType(CraftTree.Type.SeamothUpgrades)
            .WithStepsToFabricatorTab("CommonModules")
            .WithCraftingTime(7f);

        prefab.Register();

        CraftDataHandler.RemoveFromGroup(TechGroup.Resources, TechCategory.BasicMaterials, rovModule);
        CraftDataHandler.AddToGroup(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades, rovModule);
    }
}
