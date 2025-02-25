using System;
using HarmonyLib;
using GameNetcodeStuff;
using Steamworks;

namespace LethalClips.Patches;

using P = PlayerControllerB;


[HarmonyPatch(typeof(P), nameof(P.KillPlayer))]
internal class KillPlayer {

    private static void Prefix(
        P __instance,
        ref bool __state
    ) {
        __state = __instance.IsOwner && !__instance.isPlayerDead && __instance.AllowPlayerDeath();
    }

    private static void Postfix(
        bool __state,
        CauseOfDeath causeOfDeath
    ) {
        if(__state) { // check whether player actually died
            Plugin.Log.LogInfo($"Player was killed by {causeOfDeath}!");
            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                $"Killed by {Enum.GetName(typeof(CauseOfDeath), causeOfDeath)}",
                "git gud lol",
                "steam_death",
                0,
                0,
                TimelineEventClipPriority.Standard
            );
            Plugin.Log.LogInfo($"Added timeline event {timelineEvent}.");
        }
    }
}
