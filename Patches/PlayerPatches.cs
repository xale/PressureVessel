using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using xale.Subnautica.PressureVessel.Config;

namespace xale.Subnautica.PressureVessel.Patches;

[HarmonyPatch(typeof(Player))]
internal class PlayerPatches
{
    [HarmonyPatch(nameof(Player.Update)), HarmonyPostfix]
    public static void Update_Postfix(Player __instance)
    {
        if (!AvatarInputHandler.main.IsEnabled()) { return; }

        Vehicle currentVehicle = __instance.currentMountedVehicle;

        if (currentVehicle == null || currentVehicle.ignoreInput) { return; }

        if (Input.GetKeyDown(PressureVesselOptions.get().openVehicleInventoryKey))
        {
            DebugMessages.Show("keyDown: openVehicleInventoryKey");

            Exosuit prawn = (currentVehicle as Exosuit);
            if (prawn != null)
            {
                OpenPrawnStorage(prawn);
                return;
            }

            SeaMoth seamoth = (currentVehicle as SeaMoth);
            if (seamoth != null)
            {
                OpenSeamothStorage(seamoth);
                return;
            }

            DebugMessages.Show($"Unknown vehicle type: ${currentVehicle}");
            return;
        }

        if (Input.GetKeyDown(PressureVesselOptions.get().openVehicleModulesKey))
        {
            DebugMessages.Show("keyDown: openVehicleModulesKey");
            currentVehicle.upgradesInput.OpenFromExternal();
            return;
        }

        if (GameInput.GetButtonDown(GameInput.Button.Reload))
        {
            DebugMessages.Show("keyDown: reload");
            DebugMessages.Show(
                $"energyMixins count: ${currentVehicle.energyInterface.sources.Length}");
            currentVehicle.energyInterface.sources[0].InitiateReload();
        }
    }

    private static void OpenPrawnStorage(Exosuit prawn)
    {
        Inventory.main.ClearUsedStorage();
        Inventory.main.SetUsedStorage(prawn.storageContainer.container);
        Player.main.GetPDA().Open(PDATab.Inventory);
    }

    private static void OpenSeamothStorage(SeaMoth seamoth)
    {
        List<IItemsContainer> storages = new List<IItemsContainer>();
        seamoth.GetAllStorages(storages);

        Inventory.main.ClearUsedStorage();
        foreach (IItemsContainer storage in storages)
        {
            Inventory.main.SetUsedStorage(storage, /* append= */ true);
        }
        Player.main.GetPDA().Open(PDATab.Inventory);
    }
}
