using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = ButlerBeesEnemyAI;


[HarmonyPatch(typeof(P), nameof(P.OnCollideWithPlayer))]
internal class HornetsPatch {
    private static void Prefix(
        Collider other
    ) {
        var player = other.GetComponent<PlayerControllerB>();
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Electrocuted;
        death.source = "Mask Hornets";
    }
}
