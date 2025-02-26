using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = BlobAI;


[HarmonyPatch(typeof(P), nameof(P.OnCollideWithPlayer))]
internal class SlimePatch {
    private static void Prefix(
        Collider other
    ) {
        var player = other.GetComponent<PlayerControllerB>();
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Disintegrated;
        death.source = "Hydrogere";
    }
}
