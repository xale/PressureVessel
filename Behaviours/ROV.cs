using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Extensions;
using System.Collections;
using UnityEngine;
using UWE;

namespace xale.Subnautica.PressureVessel.Behaviours;

internal class ROV : MapRoomCamera, IInputHandler
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

    internal void Control()
    {
        DebugMessages.Show("ROV.Control");
        base.ControlCamera(/* MapRoomCameraScreen= */ null);
        InputHandlerStack.main.Push(this);
    }

    bool IInputHandler.HandleInput()
    {
        base.HandleInput();
        return true;
    }

    public bool HandleLateInput()
    {
        return true;
    }

    public void OnFocusChanged(InputFocusMode mode)
    {
        // No-op.
    }

    internal static IEnumerator Spawn()
    {
        CoroutineTask<GameObject> rovPrefabLoader =
            CraftData.GetPrefabForTechTypeAsync(rov, /* verbose= */ false);
        yield return rovPrefabLoader;
        GameObject rovPrefab = rovPrefabLoader.GetResult();
        DebugMessages.Show($"rovPrefab: {rovPrefab}");

        Vector3 playerPosition = Player.main.gameObject.transform.position;
        Vector3 spawnPosition = playerPosition + new Vector3(0, 0, 3);
        DebugMessages.Show($"spawnPosition: {spawnPosition}");

        ROV rovInstance =
            Instantiate(rovPrefab, spawnPosition, Player.main.transform.rotation)
                .GetComponent<ROV>();
        DebugMessages.Show($"rovInstance: {rovInstance}");
    }

    [HarmonyPatch(typeof(MapRoomCamera))]
    internal class MapRoomCameraPatches
    {
        [HarmonyPatch(nameof(MapRoomCamera.Start)), HarmonyPostfix]
        internal static void Start_Postfix(MapRoomCamera __instance)
        {
            if (__instance.GetType() != typeof(ROV)) { return; }

            // Because the static initialization for MapRoomCamera is performed asynchronously,
            // we need to wait for it to complete before we can revert it.
            CoroutineHost.StartCoroutine(WaitAndRemoveFromScannerCameraList(__instance));
        }

        private static IEnumerator WaitAndRemoveFromScannerCameraList(MapRoomCamera camera)
        {
            // Keep ROVs out of the global list of scanner-room cameras.
            yield return new WaitUntil(() => MapRoomCamera.cameras.Remove(camera));
        }
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
