using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = ButlerEnemyAI;


[HarmonyPatch(typeof(P), nameof(P.OnCollideWithPlayer))]
internal class ButlerPatch {
    private static void Prefix(
        Collider other
    ) {
        var player = other.GetComponent<PlayerControllerB>();
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Stabbed;
        death.source = "Butler";
    }
}
