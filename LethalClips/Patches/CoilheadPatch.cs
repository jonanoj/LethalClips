using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = SpringManAI;


[HarmonyPatch(typeof(P), nameof(P.OnCollideWithPlayer))]
internal class CoilheadPatch {
    private static void Prefix(
        Collider other
    ) {
        var player = other.GetComponent<PlayerControllerB>();
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Springed;
        death.source = "Coil-Head";
    }
}
