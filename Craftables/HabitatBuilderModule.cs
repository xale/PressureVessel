namespace xale.Subnautica.PressureVessel.Craftables;

/// <summary>
/// A vehicle module that allows the Seamoth and Prawn to build habitat components.
/// </summary>
internal static class HabitatBuilderModule
{
    public static TechType habitatBuilderModule { get; private set; }

    public static void Register()
    {
        habitatBuilderModule =
            VehicleModuleBuilder
                .WithTechType(
                    "HabitatBuilderModule",
                    "Habitat Builder Module",
                    "Allows fabrication of habitat compartments. Prawn/Seamoth compatible.")
                .SetIcon(SpriteManager.Get(TechType.Builder)) // TODO(xale): custom sprite
                .SetQuickSlotType(QuickSlotType.Selectable)
                .SetRecipeIngredients(
                new CraftData.Ingredient(TechType.WiringKit),
                new CraftData.Ingredient(TechType.ComputerChip),
                new CraftData.Ingredient(TechType.Titanium, 3))
                .SetFabricatorPath("CommonModules")
                .SetCraftingTimeSeconds(5f)
                .SetOnModuleUsed(
                (Vehicle vehicle, int slotId, float _charge, float _chargeScalar) =>
                {
                    DebugMessages.Show("HabitatBuilderModule.OnModuleUsed()");
                })
                .RegisterAndGetTechType();

        // TODO(xale): configure prerequisites (Seamoth or Prawn, Habitat Builder)
    }
}
