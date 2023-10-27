namespace xale.Subnautica.PressureVessel.Craftables;

internal static class BeaconDropModule
{
    public static TechType beaconDropModule { get; private set; }

    internal static void Register()
    {
        beaconDropModule =
            VehicleModuleBuilder
                .WithTechType(
                    "BeaconDropModule",
                    "Beacon Drop Tube",
                    "Allows vehicles to deploy Beacons. Prawn/Seamoth compatible")
                .SetIcon(SpriteManager.Get(TechType.Beacon)) // TODO(xale): custom sprite
                .SetQuickSlotType(QuickSlotType.Selectable)
                .SetRecipeIngredients(
                    new CraftData.Ingredient(TechType.Titanium, 3),
                    new CraftData.Ingredient(TechType.Silicone))
                .SetFabricatorPath("CommonModules")
                .SetCraftingTimeSeconds(3f)
                .SetOnModuleUsed(OnUsed)
                .RegisterAndGetTechType();
    }

    private static void OnUsed(Vehicle vehicle, int _slotId, float _charge, float _chargeScalar)
    {
        DebugMessages.Show("BeaconDropModule.OnUsed()");
    }
}
