using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = CentipedeAI;


[HarmonyPatch(typeof(P), "DamagePlayerOnIntervals")]
internal class FleaPatch {
    private static void Prefix(
        P __instance,
        ref Death __state
    ) {
        var player = __instance.clingingToPlayer;
        var death = State<Death>.Of(player);
        __state = new() {
            cause = death.cause,
            source = death.source
        };
        death.cause = TranslatedCauseOfDeath.Suffocated;
        death.source = "Snare Flea";
    }

    private static void Postfix(
        P __instance,
        Death __state
    ) {
        // restore the previous cause of death, in case no kill happened
        var player = __instance.clingingToPlayer;
        var death = State<Death>.Of(player);
        death.cause = __state.cause;
        death.source = __state.source;
    }
}
