using HarmonyLib;
using System.CodeDom;
using System.Collections.Generic;

namespace xale.Subnautica.PressureVessel.Patches.GUI;

[HarmonyPatch(typeof(PDA))]
internal static class PDAPatches
{
    [HarmonyPatch(nameof(PDA.Open)), HarmonyPrefix]
    internal static void Open_Prefix(PDA __instance)
    {
        DebugMessages.Show("Open_Prefix");
        Player player = Player.main;

        // If the player is not in a vehicle, open the PDA as normal.
        Vehicle currentVehicle = player.currentMountedVehicle;
        DebugMessages.Show($"player.currentMountedVehicle: ${currentVehicle}");
        if (currentVehicle == null) { return; }

        // If inside a Seamoth, display all attached storages alongside the player's inventory.
        SeaMoth seamoth = (currentVehicle as SeaMoth);
        if (seamoth != null)
        {
            List<IItemsContainer> storages = new List<IItemsContainer>();
            currentVehicle.GetAllStorages(storages);
            Inventory.main.ClearUsedStorage();
            foreach (IItemsContainer storage in storages)
            {
                Inventory.main.SetUsedStorage(storage, /* append= */ true);
            }
            return;
        }

        // If inside a Prawn, open the single "consolidated" storage.
        Exosuit exosuit = (currentVehicle as Exosuit);
        if (exosuit != null)
        {
            StorageContainer prawnStorage = exosuit.storageContainer;
            Inventory.main.ClearUsedStorage();
            Inventory.main.SetUsedStorage(prawnStorage.container);
            return;
        }

        DebugMessages.Show($"Unknown vehicle type: ${currentVehicle}");
    }
}
