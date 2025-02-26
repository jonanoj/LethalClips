using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = RadMechAI;


[HarmonyPatch(typeof(P), "TorchPlayerAnimation")]
internal class OldPatch_TorchPlayerAnimation {
    private static void Prefix(
        P __instance
    ) {
        var player = __instance.inSpecialAnimationWithPlayer;
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Incinerated;
        death.source = "Old Bird";
    }
}


[HarmonyPatch(typeof(P), "Stomp")]
internal class OldPatch_Stomp {
    private static void Prefix(
        ref Death __state
     ) {
        var player = GameNetworkManager.Instance.localPlayerController;
        var death = State<Death>.Of(player);
        __state = new() {
            cause = death.cause,
            source = death.source
        };
        death.cause = TranslatedCauseOfDeath.Crushed;
        death.source = "Old Bird";
    }

    private static void Postfix(
        Death __state
    ) {
        // restore the previous cause of death, in case no kill happened
        var player = GameNetworkManager.Instance.localPlayerController;
        var death = State<Death>.Of(player);
        death.cause = __state.cause;
        death.source = __state.source;
    }
}


[HarmonyPatch(typeof(P), nameof(P.SetExplosion))]
internal class OldPatch_SetExplosion {
    private static void Prefix(
        ref Death __state
     ) {
        var player = GameNetworkManager.Instance.localPlayerController;
        var death = State<Death>.Of(player);
        __state = new() {
            cause = death.cause,
            source = death.source
        };
        death.cause = TranslatedCauseOfDeath.Exploded;
        death.source = "Old Bird";
    }

    private static void Postfix(
        Death __state
    ) {
        // restore the previous cause of death, in case no kill happened
        var player = GameNetworkManager.Instance.localPlayerController;
        var death = State<Death>.Of(player);
        death.cause = __state.cause;
        death.source = __state.source;
    }
}
