using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = SandSpiderAI;


[HarmonyPatch(typeof(P), nameof(P.OnCollideWithPlayer))]
internal class SpiderPatch {
    private static void Prefix(
        Collider other
    ) {
        var player = other.GetComponent<PlayerControllerB>();
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Mauled;
        death.source = "Bunker Spider";
    }
}
