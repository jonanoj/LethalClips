using System;
using System.Collections;
using GameNetcodeStuff;
using HarmonyLib;
using LethalClips;
using Steamworks;


public static class TeleporterUtil {
    public static void CreateTeleportEvent(bool inverse, bool dead = false) {
        if(!Config.Clips.Teleporter.Value) {
            return;
        }

        // TODO: identify who triggered teleporter
        var description = $"{(inverse ? "Banish" : (dead ? "Yoink" : "Sav"))}ed by {(inverse ? " inverse" : "")} teleporter";

        try {
            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                "Teleported",
                description,
                "steam_transfer",
                0,
                0,
                TimelineEventClipPriority.Standard
            );
            Plugin.Log.LogDebug($"Added timeline event {timelineEvent}.");
        } catch(Exception e) {
            Plugin.Log.LogError($"Failed to add timeline event: {e}");
        }
    }
}


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

            if(playerScript == Player.Local) {
                TeleporterUtil.CreateTeleportEvent(inverse: false, playerScript.isPlayerDead);
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ShipTeleporter.TeleportPlayerOutWithInverseTeleporter))]
    public static void TeleportPlayerOutWithInverseTeleporter(ref IEnumerator __result, int playerObj) {
        // since the original function is a coroutine, we need to wrap the output to properly postfix
        var original = __result;
        __result = Wrapper();

        IEnumerator Wrapper() {
            yield return original;

            if(Player.FromID(playerObj) == Player.Local) {
                TeleporterUtil.CreateTeleportEvent(inverse: true);
            }
        }
    }
}
