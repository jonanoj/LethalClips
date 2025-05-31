using System.Collections;
using GameNetcodeStuff;
using HarmonyLib;
using LethalClips;



[HarmonyPatch(typeof(ShipTeleporter))]
public class TeleporterPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ShipTeleporter.beamUpPlayer))]
    public static void BeamUpPlayer(ref IEnumerator __result, PlayerControllerB playerScript) {
        // since the original function is a coroutine, we need to wrap the output to properly postfix
        var original = __result;
        __result = Wrapper();

        IEnumerator Wrapper() {
            yield return original;

            if(Config.Clips.Teleporter.Value && playerScript == Player.Local) {
                var description = $"{(playerScript.isPlayerDead ? "Yoink" : "Sav")}ed by the teleporter";
                Steam.AddEvent("Teleported", description, Steam.Icon.Transfer);
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ShipTeleporter.TeleportPlayerOutWithInverseTeleporter))]
    public static void TeleportPlayerOutWithInverseTeleporter(int playerObj) {
        if(Config.Clips.Teleporter.Value && Player.FromID(playerObj) == Player.Local) {
            Steam.AddEvent("Teleported", "Inverse teleported into the facility", Steam.Icon.Transfer);
        }
    }
}
