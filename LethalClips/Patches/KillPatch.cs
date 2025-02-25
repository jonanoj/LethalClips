using System;
using HarmonyLib;
using GameNetcodeStuff;
using Steamworks;

namespace LethalClips.Patches;

using P = PlayerControllerB;


enum ExtendedCauseOfDeath {
    Unknown,
    Bludgeoning,
    Gravity,
    Blast,
    Strangulation,
    Suffocation,
    Mauling,
    Gunshots,
    Crushing,
    Drowning,
    Abandoned,
    Electrocution,
    Kicking,
    Burning,
    Stabbing,
    Fan,
    Inertia,
    Snipped, // this and above are vanilla
    EyelessDog, // this and below are custom
    EarthLeviathan
}

internal class Death {
    internal ExtendedCauseOfDeath cause;

    internal string Cause => Enum.GetName(typeof(ExtendedCauseOfDeath), cause);

    internal string Message {
        get {
            switch(cause) {
                case ExtendedCauseOfDeath.Unknown:
                    return "Unknown cause of death.";
                case ExtendedCauseOfDeath.Drowning:
                    return "Drowned.";
                case ExtendedCauseOfDeath.Abandoned:
                    return "Left behind.";
                case ExtendedCauseOfDeath.Snipped:
                    return "Snipped.";
            }

            string pascal = Cause ?? "divine intervention";

            string spaced = "";
            foreach(char c in pascal) {
                if(char.IsUpper(c)) {
                    spaced += " ";
                }
                spaced += c;
            }

            return $"Killed by {spaced.Trim()}.";
        }
    }
}

[HarmonyPatch(typeof(P), nameof(P.KillPlayer))]
internal class KillPatch {

    private static void Prefix(
        P __instance,
        ref bool __state
    ) {
        __state = __instance.IsOwner && !__instance.isPlayerDead && __instance.AllowPlayerDeath();
    }

    private static void Postfix(
        P __instance,
        bool __state,
        CauseOfDeath causeOfDeath
    ) {
        if(__state) { // check whether player actually died
            var death = State<Death>.Of(__instance);
            if(death.cause == ExtendedCauseOfDeath.Unknown) {
                death.cause = (ExtendedCauseOfDeath)causeOfDeath;
            }

            Plugin.Log.LogInfo($"Player died! Cause of death: {death.Cause}");
            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                death.Message,
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
