using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using System.Collections.Generic;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;

namespace xale.Subnautica.PressureVessel.Craftables;

/// <summary>
/// A vehicle module that allows the Seamoth and Prawn to scan objects in the environment.
/// </summary>
internal static class ScannerModule
{
    public static TechType scannerModule { get; private set; }

    public static void Register()
    {
        PrefabInfo info =
            PrefabInfo
                .WithTechType(
                    "ScannerModule",
                    "Scanner Module",
                    "Allows vehicles to scan objects for the databank. Prawn/Seamoth compatible.")
                .WithIcon(SpriteManager.Get(TechType.Scanner)); // TODO(xale): custom sprite
        scannerModule = info.TechType;


        CustomPrefab prefab = new CustomPrefab(info);
        prefab
            .SetVehicleUpgradeModule(EquipmentType.VehicleModule, QuickSlotType.Selectable)
            .WithOnModuleUsed(
                (Vehicle vehicle, int slotId, float _charge, float _chargeScalar) =>
                {
                    DebugMessages.Show("ScannerModule.OnModuleUsed()");
                });

        CloneTemplate template = new CloneTemplate(info, TechType.VehicleStorageModule);
        prefab.SetGameObject(template);

        // TODO(xale): configure prerequisites (Seamoth or Prawn)

        RecipeData recipe = new RecipeData()
        {
            Ingredients = new List<CraftData.Ingredient>() {
                new CraftData.Ingredient(TechType.ComputerChip),
                new CraftData.Ingredient(TechType.Titanium, 3),
            },
            craftAmount = 1,
        };
        prefab.SetRecipe(recipe)
            .WithFabricatorType(CraftTree.Type.SeamothUpgrades)
            .WithStepsToFabricatorTab("CommonModules")
            .WithCraftingTime(5f);

        prefab.Register();

        CraftDataHandler.RemoveFromGroup(
            TechGroup.Resources,
            TechCategory.BasicMaterials,
            scannerModule);
        CraftDataHandler.AddToGroup(
            TechGroup.VehicleUpgrades,
            TechCategory.VehicleUpgrades,
            scannerModule);
    }
}
