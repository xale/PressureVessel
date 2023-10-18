using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Extensions;
using System.Collections;
using UnityEngine;

namespace xale.Subnautica.PressureVessel.Behaviours;

internal class ROV : MapRoomCamera
{
    public static TechType rov { get; private set; }

    internal static void RegisterPrefab()
    {
        CustomPrefab rovPrefab =
            new CustomPrefab("ROV", "RemOra Drone", "Remotely-operated utility drone.");
        rov = rovPrefab.Info.TechType;

        CloneTemplate template = new CloneTemplate(rovPrefab.Info, TechType.MapRoomCamera);
        template.ModifyPrefab += instance =>
        {
            instance.GetComponent<Pickupable>().isPickupable = false;

            MapRoomCamera baseBehavior = instance.GetComponent<MapRoomCamera>();
            instance.AddComponent<ROV>().CopyComponent(baseBehavior);
            GameObject.DestroyImmediate(baseBehavior);

            // TODO(xale): configure docking point on mothership
            instance.EnsureComponent<ROVDocking>();
            instance.EnsureComponent<DummyMapRoomScreen>();
        };

        // TODO(xale): set custom model

        rovPrefab.SetGameObject(template);

        rovPrefab.Register();
    }

    internal static IEnumerator SpawnAndControl()
    {
        CoroutineTask<GameObject> rovPrefabLoader =
            CraftData.GetPrefabForTechTypeAsync(rov, /* verbose= */ false);
        yield return rovPrefabLoader;
        GameObject rovPrefab = rovPrefabLoader.GetResult();
        DebugMessages.Show($"rovPrefab: ${rovPrefab}");

        Vector3 playerPosition = Player.main.gameObject.transform.position;

        // TODO(xale): check valid spawn position
        Vector3 spawnPosition = playerPosition + new Vector3(0, 0, 3);
        DebugMessages.Show($"spawnPosition: ${spawnPosition}");

        ROV rovInstance =
            Instantiate(rovPrefab, spawnPosition, Player.main.transform.rotation)
                .GetComponent<ROV>();
        DebugMessages.Show($"rovInstance: ${rovInstance}");

        // TODO(xale): configure docking point on mothership
        rovInstance.dockingPoint = rovInstance.GetComponent<ROVDocking>();
        DebugMessages.Show($"dockingPoint: ${rovInstance.dockingPoint}");

        DebugMessages.Show($"rovInstance.CanBeControlled: ${rovInstance.CanBeControlled()}");

        rovInstance.ControlCamera(rovInstance.GetComponent<DummyMapRoomScreen>());
    }

    [HarmonyPatch(typeof(MapRoomCamera))]
    internal static class MapRoomCameraRovPatches
    {
        [HarmonyPatch(nameof(MapRoomCamera.UpdateEnergyRecharge)), HarmonyPrefix]
        internal static bool UpdateEnergyRecharge_Prefix(MapRoomCamera __instance)
        {
            DebugMessages.Show("UpdateEnergyRecharge_Prefix");
            ROV rovInstance = (__instance as ROV);
            DebugMessages.Show($"rovInstance: ${rovInstance}");
            if (rovInstance == null ) { return true; } // Use default behavior for normal cameras.

            // TODO(xale): consume power from mothership
            return false;
        }
    }
    internal class DummyMapRoomScreen : MapRoomScreen { }

    [HarmonyPatch(typeof(MapRoomScreen))]
    internal static class MapRoomScreenDummyPatches
    {
        [HarmonyPatch(nameof(MapRoomScreen.OnCameraFree)), HarmonyPrefix]
        internal static bool OnCameraFree_Prefix(MapRoomScreen __instance)
        {
            // Run only if this is *not* a dummy screen.
            return (__instance.GetType() != typeof(DummyMapRoomScreen));
        }
    }
}
