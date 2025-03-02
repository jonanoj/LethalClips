using System;
using HarmonyLib;
using GameNetcodeStuff;
using Steamworks;
using UnityEngine;

namespace LethalClips.Patches;

using P = PlayerControllerB;


enum TranslatedCauseOfDeath {
    Killed, // Unknown
    Bludgeoned, // Bludgeoning
    SPLAT, // Gravity
    Blast, // Blast
    Strangled, // Strangulation
    Suffocated, // Suffocation
    Mauled, // Mauling
    Shot, // Gunshots
    Crushed, // Crushing
    Drowned, // Drowning
    Abandoned, // Abandoned
    Electrocuted, // Electrocution
    Kicked, // Kicking
    Incinerated, // Burning
    Stabbed, // Stabbing
    Sliced, // Fan
    Crashed, // Inertia
    Snipped, // Snipped
    Devoured,
    Springed,
    Died,
    Disintegrated,
    Infected,
    Exploded,
    Embarrassing
}


[HarmonyPatch(typeof(P), nameof(P.KillPlayer))]
internal static class KillPatch {

    private static TranslatedCauseOfDeath cause;
    private static string source;
    private static float time;

    internal static string Message {
        get {
            string message = Enum.GetName(typeof(TranslatedCauseOfDeath), cause) ?? "Killed";
            if(!string.IsNullOrEmpty(source)) {
                message += " by " + source;
            }
            return message;
        }
    }

    internal static P Player => GameNetworkManager.Instance.localPlayerController;

    internal static void Kill(TranslatedCauseOfDeath cause, string source, float timeout = 0.1f) {
        KillPatch.cause = cause;
        KillPatch.source = source;
        time = Time.time + timeout;
    }

    private static void Prefix(
        P __instance,
        ref bool __state
    ) {
        // the body of the method changes these values, so cache them now
        __state = __instance.IsOwner && !__instance.isPlayerDead && __instance.AllowPlayerDeath();
    }

    private static void Postfix(
        P __instance,
        bool __state,
        CauseOfDeath causeOfDeath
    ) {
        // use stored value to determine if we actually need to do anything
        if(__state) {
            if(Time.time > time || cause == TranslatedCauseOfDeath.Killed) {
                cause = (TranslatedCauseOfDeath)causeOfDeath;
                source = "";
            }

            Plugin.Log.LogInfo($"Player died! Cause of death: {Message}");
            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                Message,
                "git gud lol",
                "steam_death",
                0,
                0,
                TimelineEventClipPriority.Standard
            );
            Plugin.Log.LogInfo($"Added timeline event {timelineEvent}.");

            time = 0; // reset cause of death
        }
    }
}
