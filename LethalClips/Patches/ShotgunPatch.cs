using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = ShotgunItem;


[HarmonyPatch(typeof(P), "ShootGun")]
internal class ShotgunPatch {
    private static void Prefix(
        P __instance,
        ref Death __state
    ) {
        var player = GameNetworkManager.Instance.localPlayerController;
        var death = State<Death>.Of(player);
        __state = new() {
            cause = death.cause,
            source = death.source
        };
        death.cause = TranslatedCauseOfDeath.Shot;
        if(__instance.isHeldByEnemy) {
            death.source = "Nutcracker";
        } else if(__instance.isHeld) {
            death.source = __instance.playerHeldBy?.playerUsername ?? "Player";
        } else {
            death.source = "accident";
        }
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

