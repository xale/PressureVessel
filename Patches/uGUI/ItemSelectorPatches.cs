using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xale.Subnautica.PressureVessel.Patches.uGUI;

[HarmonyPatch(typeof(uGUI_ItemSelector))]
internal class ItemSelectorPatches
{
    [HarmonyPatch(nameof(uGUI_ItemSelector.HandleInput)), HarmonyPrefix]
    internal static bool HandleInput_Prefix(uGUI_ItemSelector __instance, ref bool __result)
    {
        // If the player presses the "reload" button while accessing a power cell in a vehicle with
        // more than one power cell, switch to the next power cell rather than closing.
        if (GameInput.GetButtonDown(GameInput.Button.Reload))
        {
            Vehicle currentVehicle = Player.main.currentMountedVehicle;
            if (currentVehicle == null) { return true; }

            EnergyMixin[] energySources = currentVehicle.energyInterface.sources;
            int currentIndex = Array.IndexOf(energySources, __instance.manager);
            if (currentIndex < 0 || currentIndex == (energySources.Length - 1)) { return true; }

            energySources[currentIndex + 1].InitiateReload();

            __result = true; // (Keep selector open.)
            return false;
        }

        // Continue to standard input handling.
        return true;
    }
}
