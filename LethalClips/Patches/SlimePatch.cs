using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = BlobAI;


[HarmonyPatch(typeof(P), nameof(P.OnCollideWithPlayer))]
internal class SlimePatch {
    private static void Prefix(
        Collider other,
        ref Death __state
    ) {
        var player = other.GetComponent<PlayerControllerB>();
        var death = State<Death>.Of(player);
        __state = new() {
            cause = death.cause,
            source = death.source
        };
        death.cause = TranslatedCauseOfDeath.Disintegrated;
        death.source = "Hydrogere";
    }

    private static void Postfix(
        Collider other,
        Death __state
    ) {
        // restore the previous cause of death, in case no kill happened
        var player = other.GetComponent<PlayerControllerB>();
        var death = State<Death>.Of(player);
        death.cause = __state.cause;
        death.source = __state.source;
    }
}
