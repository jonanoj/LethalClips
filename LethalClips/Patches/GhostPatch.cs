using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = DressGirlAI;


[HarmonyPatch(typeof(P), nameof(P.OnCollideWithPlayer))]
internal class GhostPatch {
    private static void Prefix(
        Collider other
    ) {
        var player = other.GetComponent<PlayerControllerB>();
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Died;
        death.source = "";
    }
}
