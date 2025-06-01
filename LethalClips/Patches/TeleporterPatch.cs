using HarmonyLib;
using System.Collections;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(ShipTeleporter))]
public class TeleporterPatch {
    [HarmonyPatch(nameof(ShipTeleporter.beamUpPlayer))]
    [HarmonyPostfix]
    public static void BeamUpPlayer(ref IEnumerator __result) {
        // since the original function is a coroutine, we need to wrap the output to properly postfix
        var original = __result;
        __result = Wrapper();

        IEnumerator Wrapper() {
            // save this before the coroutine runs
            var player = StartOfRound.Instance.mapScreen.targetedPlayer;

            yield return original;

            if(Config.Clips.Teleporter.Value && player == Player.Local) {
                var description = $"{(player.isPlayerDead ? "Yoink" : "Sav")}ed by the teleporter";
                Steam.AddEvent("Teleported", description, Steam.Icon.Transfer);
            }
        }
    }

    [HarmonyPatch(nameof(ShipTeleporter.TeleportPlayerOutWithInverseTeleporter))]
    [HarmonyPostfix]
    public static void TeleportPlayerOutWithInverseTeleporter(int playerObj) {
        if(Config.Clips.Teleporter.Value && Player.FromID(playerObj) == Player.Local) {
            Steam.AddEvent("Teleported", "Inverse teleported into the facility", Steam.Icon.Transfer);
        }
    }
}
