using BepInEx.Logging;
using HarmonyLib;
using xale.Subnautica.PressureVessel.Config;

namespace xale.Subnautica.PressureVessel.Patches.Vehicles;

[HarmonyPatch(typeof(Vehicle))]
internal static class AllVehiclesPatches
{
    [HarmonyPatch(nameof(Vehicle.GetAllowedToEject)), HarmonyPrefix]
    private static bool GetAllowedToEject_Prefix(Vehicle __instance, ref bool __result)
    {
        ErrorMessage.AddDebug("GetAllowedToEject_Prefix");
        bool lockHatches = PressureVesselOptions.get().lockHatches;
        ErrorMessage.AddDebug($"lockHatches: {lockHatches}");

        if (!lockHatches) { return true; }

        Vehicle vehicle = __instance;

        int currentDepth;
        vehicle.GetDepth(out currentDepth, out _);

        ErrorMessage.AddDebug($"currentDepth: {currentDepth}");

        int safeDepth = PressureVesselConfig.SafeDepth.Value;

        ErrorMessage.AddDebug($"safeDepth: {safeDepth}");

        if (currentDepth <= PressureVesselConfig.SafeDepth.Value) { return true; }

        ErrorMessage.AddError("Cannot open hatch; pressure is too high");

        __result = false;
        return false;
    }
}
