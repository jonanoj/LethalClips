using System;
using HarmonyLib;
using GameNetcodeStuff;
using Steamworks;
using UnityEngine;

namespace LethalClips.Patches;

using P = PlayerControllerB;


[HarmonyPatch(typeof(P), nameof(P.KillPlayer))]
internal static class KillPatch {

    private static ExtendedCauseOfDeath cause;
    private static string source;
    private static float time;

    internal static string Message {
        get {
            string message = Enum.GetName(typeof(ExtendedCauseOfDeath), cause) ?? "Killed";
            if(!string.IsNullOrEmpty(source)) {
                message += " by " + source;
            }
            return message;
        }
    }

    internal static P Player => GameNetworkManager.Instance.localPlayerController;

    internal static void Damage(ExtendedCauseOfDeath cause, string source, float damage) {
        if(Player.health <= damage && (damage > 50 || Player.criticallyInjured)) {
            Kill(cause, source);
        }
    }

    internal static void Kill(ExtendedCauseOfDeath cause, string source, float timeout = 0.1f) {
        if(time < 0) {
            return; // player already has an inevitable cause of death registered
        }
        KillPatch.cause = cause;
        KillPatch.source = source;
        if(timeout >= 0) {
            time = Time.time + timeout;
        } else {
            time = -1; // signals that this death is inevitable; nothing else can modify cause of death anymore
        }
    }

    private static void Prefix(
        P __instance,
        ref bool __state
    ) {
        // the body of the method changes these values, so cache them now
        __state = __instance.IsOwner && !__instance.isPlayerDead && __instance.AllowPlayerDeath();
    }

    private static void Postfix(
        bool __state,
        CauseOfDeath causeOfDeath
    ) {
        // use stored value to determine if we actually need to do anything
        if(__state) {
            if(0 <= time && time < Time.time) {
                cause = (ExtendedCauseOfDeath)causeOfDeath;
                source = "";
            }

            time = 0; // reset cause of death

            Plugin.Log.LogInfo($"Player died! Cause of death: {Message}");

            if(!Config.Clips.Deaths.Value) {
                return;
            }
            
            try
            {
                var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                    "You died!",
                    Message,
                    "steam_death",
                    0,
                    0,
                    TimelineEventClipPriority.Standard
                );
                Plugin.Log.LogInfo($"Added timeline event {timelineEvent}.");
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Failed to add timeline event: {e}");
            }
        }
    }
}
