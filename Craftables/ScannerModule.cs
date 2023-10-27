namespace xale.Subnautica.PressureVessel.Craftables;

/// <summary>
/// A vehicle module that allows the Seamoth and Prawn to scan objects in the environment.
/// </summary>
internal static class ScannerModule
{
    public static TechType scannerModule { get; private set; }

    public static void Register()
    {
        scannerModule =
            VehicleModuleBuilder
                .WithTechType(
                    "ScannerModule",
                    "Scanner Module",
                    "Allows vehicles to scan objects for the databank. Prawn/Seamoth compatible.")
                .SetIcon(SpriteManager.Get(TechType.Scanner)) // TODO(xale): custom sprite
                .SetQuickSlotType(QuickSlotType.Selectable)
                .SetRecipeIngredients(
                    new CraftData.Ingredient(TechType.ComputerChip),
                    new CraftData.Ingredient(TechType.Titanium, 3))
                .SetFabricatorPath("CommonModules")
                .SetCraftingTimeSeconds(5f)
                .SetOnModuleUsed(
                (Vehicle vehicle, int slotId, float _charge, float _chargeScalar) =>
                {
                    DebugMessages.Show("ScannerModule.OnModuleUsed()");
                })
                .RegisterAndGetTechType();

        // TODO(xale): configure prerequisites (Seamoth or Prawn)
    }
}
