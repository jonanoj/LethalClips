using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = SpikeRoofTrap;


[HarmonyPatch(typeof(P), nameof(P.OnTriggerStay))]
internal static class SpikePatch {
    private static void Postfix(
        P __instance,
        Collider other
    ) {
        if(!__instance.trapActive || !__instance.slammingDown || Time.realtimeSinceStartup - __instance.timeSinceMovingUp < 0.75f) {
            return;
        }

        PlayerControllerB component = other.gameObject.GetComponent<PlayerControllerB>();
        if(component == KillPatch.Player && !component.isPlayerDead) {
            KillPatch.Kill(TranslatedCauseOfDeath.Crushed, "Spike Trap");
        }
    }
}
