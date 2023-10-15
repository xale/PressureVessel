using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System.Collections.Generic;

namespace xale.Subnautica.PressureVessel.Craftables;

/// <summary>
/// A vehicle module that allows the Seamoth and Prawn to build habitat components.
/// </summary>
internal static class HabitatBuilderModule
{
    public static TechType habitatBuilderModule { get; private set; }

    public static void Register()
    {
        PrefabInfo info =
            PrefabInfo
                .WithTechType(
                    "HabitatBuilderModule",
                    "Habitat Builder Module",
                    "Allows fabrication of habitat compartments. Prawn/Seamoth compatible.")
                .WithIcon(SpriteManager.Get(TechType.Builder)); // TODO(xale): custom sprite
        habitatBuilderModule = info.TechType;

        CustomPrefab prefab = new CustomPrefab(info);
        prefab
            .SetVehicleUpgradeModule(EquipmentType.VehicleModule, QuickSlotType.Selectable)
            .WithOnModuleAdded((Vehicle vehicle, int slotId) =>
            {
                DebugMessages.Show("HabitatBuilderModule.OnModuleAdded()");
            })
            .WithOnModuleUsed(
                (Vehicle vehicle, int slotId, float _charge, float _chargeScalar) =>
                {
                    DebugMessages.Show("HabitatBuilderModule.OnModuleUsed()");
                });

        CloneTemplate template = new CloneTemplate(info, TechType.VehicleStorageModule);
        prefab.SetGameObject(template);

        // TODO(xale): configure prerequisites (Seamoth or Prawn, Habitat Builder)

        RecipeData recipe = new RecipeData()
        {
            Ingredients = new List<CraftData.Ingredient>() {
                new CraftData.Ingredient(TechType.WiringKit),
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
            habitatBuilderModule);
        CraftDataHandler.AddToGroup(
            TechGroup.VehicleUpgrades,
            TechCategory.VehicleUpgrades,
            habitatBuilderModule);
    }
}
