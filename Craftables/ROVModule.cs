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
            .SetVehicleUpgradeModule(EquipmentType.VehicleModule, QuickSlotType.Selectable)
            .WithOnModuleAdded((Vehicle vehicle, int slotId) =>
            {
                SphereCollider dockingPort = vehicle.gameObject.AddComponent<SphereCollider>();
                dockingPort.center = new Vector3(0, -1.5f, 0);
                dockingPort.radius = 1;
                dockingPort.isTrigger = true;

                ROVDocking dockingBehavior = vehicle.gameObject.AddComponent<ROVDocking>();
                dockingBehavior.dockingPort = dockingPort;
                dockingBehavior.dockingTransform = dockingPort.transform;
                dockingBehavior.undockTransform = dockingPort.transform;

                // TODO(xale): add model for docking port
                // TODO(xale): remove drone from player's inventory, if present, and dock
            })
            .WithOnModuleRemoved((Vehicle vehicle, int _slotId) =>
            {
                // TODO(xale): (beforehand:) check space in player's inventory
                // TODO(xale): move drone, if docked, to player's inventory

                ROVDocking dockingPort = vehicle.gameObject.GetComponent<ROVDocking>();
                if (dockingPort != null) {
                    GameObject.Destroy(dockingPort.dockingPort);
                    GameObject.Destroy(dockingPort);
                }
            })
            .WithOnModuleUsed(
                (Vehicle vehicle, int slotId, float _charge, float _chargeScalar) =>
                {
                    DebugMessages.Show("RovModule.OnModuleUsed()");
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
