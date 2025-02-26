using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = ShotgunItem;


[HarmonyPatch(typeof(P), "ShootGun")]
internal class ShotgunPatch {
    private static void Prefix(
        P __instance
    ) {
        var player = GameNetworkManager.Instance.localPlayerController;
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Shot;
        if(__instance.isHeldByEnemy) {
            death.source = "Nutcracker";
        } else if(__instance.isHeld) {
            death.source = __instance.playerHeldBy?.playerUsername ?? "Player";
        } else {
            death.source = "Accident";
        }
    }
}

