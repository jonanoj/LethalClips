using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = CaveDwellerAI;


[HarmonyPatch(typeof(P), nameof(P.KillPlayerAnimationClientRpc))]
internal class ManeaterPatch {
    private static void Prefix(
        int playerObjectId
    ) {
        var player = StartOfRound.Instance.allPlayerScripts[playerObjectId];
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Mauled;
        death.source = "Maneater";
    }
}
