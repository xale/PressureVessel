using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Extensions;
using System.Collections;
using UnityEngine;
using UWE;

namespace xale.Subnautica.PressureVessel.Behaviours;

internal class ROV : MapRoomCamera
{
    public static TechType rov { get; private set; }

    internal static void RegisterPrefab()
    {
        PrefabInfo prefabInfo =
            PrefabInfo.WithTechType(
               "ROV", "RemOra Drone", "Remotely-operated utility drone.")
            .WithSizeInInventory(new Vector2int(2, 2))
            .WithIcon(SpriteManager.Get(TechType.MapRoomCamera)); // TODO(xale): custom sprite
        rov = prefabInfo.TechType;

        CloneTemplate template = new CloneTemplate(prefabInfo, TechType.MapRoomCamera);
        template.ModifyPrefab += instance =>
        {
            MapRoomCamera baseBehavior = instance.GetComponent<MapRoomCamera>();
            instance.AddComponent<ROV>().CopyComponent(baseBehavior);
            GameObject.DestroyImmediate(baseBehavior);

            instance.EnsureComponent<DummyMapRoomScreen>();

            CoroutineHost.StartCoroutine(
                instance.GetComponent<EnergyMixin>()
                    .SpawnDefaultAsync(1.0f, DiscardTaskResult<bool>.Instance));
        };

        // TODO(xale): set custom model

        CustomPrefab rovPrefab = new CustomPrefab(prefabInfo);
        rovPrefab.SetGameObject(template);

        rovPrefab.Register();
    }

    internal static IEnumerator Spawn()
    {
        CoroutineTask<GameObject> rovPrefabLoader =
            CraftData.GetPrefabForTechTypeAsync(rov, /* verbose= */ false);
        yield return rovPrefabLoader;
        GameObject rovPrefab = rovPrefabLoader.GetResult();
        DebugMessages.Show($"rovPrefab: ${rovPrefab}");

        Vector3 playerPosition = Player.main.gameObject.transform.position;
        Vector3 spawnPosition = playerPosition + new Vector3(0, 0, 3);
        DebugMessages.Show($"spawnPosition: ${spawnPosition}");

        ROV rovInstance =
            Instantiate(rovPrefab, spawnPosition, Player.main.transform.rotation)
                .GetComponent<ROV>();
        DebugMessages.Show($"rovInstance: ${rovInstance}");
    }

    internal class DummyMapRoomScreen : MapRoomScreen
    {
        [HarmonyPatch(typeof(MapRoomScreen))]
        internal static class MapRoomScreenDummyPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(MapRoomScreen.Start))]
            [HarmonyPatch(nameof(MapRoomScreen.OnCameraFree))]
            internal static bool DummyMethod_Prefix(MapRoomScreen __instance)
            {
                // Run only if this is *not* a dummy screen.
                return (__instance.GetType() != typeof(DummyMapRoomScreen));
            }
        }
    }

}
