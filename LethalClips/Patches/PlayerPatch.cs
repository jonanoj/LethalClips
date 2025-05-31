using System;
using HarmonyLib;
using GameNetcodeStuff;
using Steamworks;
using UnityEngine;

namespace LethalClips.Patches;


public class PlayerState : State<PlayerControllerB, PlayerState> {
    public static PlayerState Local => Of(Player.Local);
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

    public void TriggerDeathEvent(CauseOfDeath causeOfDeath) {
        if(0 <= deathTimeout && deathTimeout < Time.time) {
            // the cached cause of death has expired, so use the vanilla values
            this.causeOfDeath = (ExtendedCauseOfDeath)causeOfDeath;
            sourceOfDeath = "";
        }

        deathTimeout = 0; // reset cause of death
        if(Config.Clips.Deaths.Value) {
            Steam.AddEvent("You died!", Message, Steam.Icon.Death, priority: 96);
        }
    }
}


[HarmonyPatch(typeof(PlayerControllerB))]
public static class PlayerPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerControllerB.KillPlayer))]
    private static void KillPlayer(PlayerControllerB __instance, CauseOfDeath causeOfDeath) {
        // the body of the method changes these values, so this needs to be a prefix
        if(__instance.IsOwner && !__instance.isPlayerDead && __instance.AllowPlayerDeath()) {
            PlayerState.Of(__instance).TriggerDeathEvent(causeOfDeath);
        }
    }
}
