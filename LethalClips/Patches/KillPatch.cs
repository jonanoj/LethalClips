using System;
using HarmonyLib;
using GameNetcodeStuff;
using Steamworks;
using UnityEngine;

namespace LethalClips.Patches;


public class KillState : State<PlayerControllerB, KillState> {
    public static KillState Player => Of(GameNetworkManager.Instance.localPlayerController);
    public ExtendedCauseOfDeath causeOfDeath;
    public string sourceOfDeath;
    public float deathTimeout;

    public string Message {
        get {
            string message = Enum.GetName(typeof(ExtendedCauseOfDeath), causeOfDeath) ?? "Killed";
            if(!string.IsNullOrEmpty(sourceOfDeath)) {
                message += " by " + sourceOfDeath;
            }
            return message;
        }
    }

    public void Kill(ExtendedCauseOfDeath cause, string source, float timeout = 0.1f) {
        if(deathTimeout < 0) {
            return; // player already has an inevitable cause of death registered
        }

        causeOfDeath = cause;
        sourceOfDeath = source;

        if(timeout >= 0) {
            deathTimeout = Time.time + timeout; // the time at which this cause of death expires
        } else {
            deathTimeout = -1; // signals that this death is inevitable; nothing else can modify cause of death anymore
        }
    }

    public void Damage(ExtendedCauseOfDeath cause, string source, float damage) {
        if(Instance.health <= damage && (damage > 50 || Instance.criticallyInjured)) {
            Kill(cause, source);
        }
    }

    public void AddMarker(CauseOfDeath causeOfDeath) {
        if(0 <= deathTimeout && deathTimeout < Time.time) {
            // the cached cause of death has expired, so use the vanilla values
            this.causeOfDeath = (ExtendedCauseOfDeath)causeOfDeath;
            sourceOfDeath = "";
        }

        Plugin.Log.LogInfo($"Player died! Cause of death: {Message}");
        deathTimeout = 0; // reset cause of death

        try {
            if(!Config.Clips.Deaths.Value) {
                return;
            }

            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                "You died!",
                Message,
                "steam_death",
                0,
                0,
                TimelineEventClipPriority.Standard
            );
            Plugin.Log.LogInfo($"Added timeline event {timelineEvent}.");
        } catch(Exception e) {
            Plugin.Log.LogError($"Failed to add timeline event: {e}");
        }
    }
}


[HarmonyPatch(typeof(PlayerControllerB))]
public static class KillPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerControllerB.KillPlayer))]
    private static void KillPlayer(PlayerControllerB __instance, CauseOfDeath causeOfDeath) {
        // the body of the method changes these values, so this needs to be a prefix
        if(__instance.IsOwner && !__instance.isPlayerDead && __instance.AllowPlayerDeath()) {
            KillState.Of(__instance).AddMarker(causeOfDeath);
        }
    }
}
