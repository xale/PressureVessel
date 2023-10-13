using BepInEx.Logging;
using HarmonyLib;
using System;
using xale.Subnautica.PressureVessel.Config;

namespace xale.Subnautica.PressureVessel.Patches.Vehicles;

[HarmonyPatch(typeof(Vehicle))]
internal static class AllVehiclesPatches
{
    [HarmonyPatch(nameof(Vehicle.GetAllowedToEject)), HarmonyPostfix]
    static bool GetAllowedToEject_Postfix(bool originalResult, Vehicle __instance)
    {
        ErrorMessage.AddDebug($"GetAllowedToEject_Postfix; originalResult: ${originalResult}");

        // If the player isn't allowed to exit anyway, there's no need for further checks.
        if (originalResult == false) { return false; }

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

        ErrorMessage.AddError("Pressure is too high to open hatch");

        return false;
    }
}
