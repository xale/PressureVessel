using HarmonyLib;
using xale.Subnautica.PressureVessel.Config;

namespace xale.Subnautica.PressureVessel.Patches.Vehicles;

[HarmonyPatch(typeof(Vehicle))]
internal static class AllVehiclesPatches
{
    [HarmonyPatch(nameof(Vehicle.GetAllowedToEject)), HarmonyPostfix]
    internal static bool GetAllowedToEject_Postfix(bool originalResult, Vehicle __instance)
    {
        DebugMessages.Show($"GetAllowedToEject_Postfix; originalResult: ${originalResult}");

        // If the player isn't allowed to exit anyway, there's no need for further checks.
        if (originalResult == false) { return false; }

        bool lockHatches = PressureVesselConfig.LockHatches.Value;
        DebugMessages.Show($"lockHatches: {lockHatches}");

        if (!lockHatches) { return true; }

        Vehicle vehicle = __instance;

        int currentDepth;
        vehicle.GetDepth(out currentDepth, out _);
        DebugMessages.Show($"currentDepth: {currentDepth}");

        int safeDepth = PressureVesselConfig.SafeDepth.Value;
        DebugMessages.Show($"safeDepth: {safeDepth}");

        if (currentDepth <= PressureVesselConfig.SafeDepth.Value) { return true; }

        ErrorMessage.AddError("Pressure is too high to open hatch");

        return false;
    }
}
