using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = CaveDwellerAI;


[HarmonyPatch(typeof(P), nameof(P.KillPlayerAnimationClientRpc))]
internal class ManeaterPatch {
    private static void Prefix(
        int playerObjectId,
        ref Death __state
    ) {
        var player = StartOfRound.Instance.allPlayerScripts[playerObjectId];
        var death = State<Death>.Of(player);
        __state = new() {
            cause = death.cause,
            source = death.source
        };
        death.cause = TranslatedCauseOfDeath.Mauled;
        death.source = "Maneater";
    }

    private static void Postfix(
        int playerObjectId,
        Death __state
    ) {
        // restore the previous cause of death, in case no kill happened
        var player = StartOfRound.Instance.allPlayerScripts[playerObjectId];
        var death = State<Death>.Of(player);
        death.cause = __state.cause;
        death.source = __state.source;
    }
}
