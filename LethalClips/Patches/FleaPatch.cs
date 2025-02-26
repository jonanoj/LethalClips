using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = CentipedeAI;


[HarmonyPatch(typeof(P), "DamagePlayerOnIntervals")]
internal class FleaPatch {
    private static void Prefix(
        P __instance
    ) {
        var player = __instance.clingingToPlayer;
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Suffocated;
        death.source = "Snare Flea";
    }
}
