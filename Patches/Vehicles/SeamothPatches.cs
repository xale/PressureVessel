using HarmonyLib;

namespace xale.Subnautica.PressureVessel.Patches.Vehicles;

[HarmonyPatch(typeof(SeaMoth))]
internal static class SeamothPatches
{
    // The Seamoth class overrides GetAllowedToEject, and must use a separate patch.
    [HarmonyPatch(nameof(SeaMoth.GetAllowedToEject)), HarmonyPostfix]
    static bool GetAllowedToEject_Postfix(bool originalResult, SeaMoth __instance) {
        return AllVehiclesPatches.GetAllowedToEject_Postfix(originalResult, __instance);
    }
}
