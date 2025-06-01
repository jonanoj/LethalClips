using HarmonyLib;
using GameNetcodeStuff;
using System;
using UnityEngine;

namespace LethalClips.Patches;


public class PlayerState : State<PlayerControllerB, PlayerState> {
    public static PlayerState Local => Of(Player.Local);
    public ExtendedCauseOfDeath CauseOfDeath { get; private set; }
    public string SourceOfDeath { get; private set; }
    public float DeathTimeout { get; private set; }

    public string Message {
        get {
            string message = Enum.GetName(typeof(ExtendedCauseOfDeath), CauseOfDeath) ?? "Killed";
            if(!string.IsNullOrEmpty(SourceOfDeath)) {
                message += " by " + SourceOfDeath;
            }
            return message;
        }
    }

    public void Kill(ExtendedCauseOfDeath cause, string source, float timeout = 0.1f) {
        if(DeathTimeout < 0) {
            return; // player already has an inevitable cause of death registered
        }

        CauseOfDeath = cause;
        SourceOfDeath = source;

        if(timeout >= 0) {
            DeathTimeout = Time.time + timeout; // the time at which this cause of death expires
        } else {
            DeathTimeout = -1; // signals that this death is inevitable; nothing else can modify cause of death anymore
        }
    }

    public void Damage(ExtendedCauseOfDeath cause, string source, float damage) {
        if(Instance.health <= damage && (damage >= 50 || Instance.criticallyInjured)) {
            Kill(cause, source);
        }
    }

    public void TriggerDeathEvent(CauseOfDeath causeOfDeath) {
        if(0 <= DeathTimeout && DeathTimeout < Time.time) {
            // the cached cause of death has expired, so use the vanilla values
            CauseOfDeath = (ExtendedCauseOfDeath)causeOfDeath;
            SourceOfDeath = "";
        }

        DeathTimeout = 0; // reset cause of death
        if(Config.Clips.Deaths.Value) {
            Steam.AddEvent("You died!", Message, Steam.Icon.Death, priority: 96);
        }
    }
}


[HarmonyPatch(typeof(PlayerControllerB))]
public static class PlayerPatch {
    [HarmonyPatch(nameof(PlayerControllerB.KillPlayer))]
    [HarmonyPrefix]
    public static void KillPlayer(PlayerControllerB __instance, CauseOfDeath causeOfDeath) {
        // the body of the method changes these values, so this needs to be a prefix
        if(__instance.IsOwner && !__instance.isPlayerDead && __instance.AllowPlayerDeath()) {
            PlayerState.Of(__instance).TriggerDeathEvent(causeOfDeath);
        }
    }
}
