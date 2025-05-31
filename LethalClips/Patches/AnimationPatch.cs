﻿using GameNetcodeStuff;
using HarmonyLib;

namespace LethalClips.Patches;


[HarmonyPatch()]
internal static class AnimationPatch {

    [HarmonyPatch(typeof(FlowermanAI), "killAnimation")]
    [HarmonyPrefix]
    private static void Bracken(FlowermanAI __instance) {
        if(__instance.inSpecialAnimationWithPlayer == KillPatch.Player) {
            KillPatch.Kill(ExtendedCauseOfDeath.Strangled, "Bracken", -1);
        }
    }

    [HarmonyPatch(typeof(MouthDogAI), "KillPlayer")]
    [HarmonyPrefix]
    private static void EyelessDog(int playerID) {
        if(0 <= playerID && playerID < StartOfRound.Instance.allPlayerScripts.Length && StartOfRound.Instance.allPlayerScripts[playerID] == KillPatch.Player) {
            KillPatch.Kill(ExtendedCauseOfDeath.Mauled, "Eyeless Dog", -1);
        }
    }
    
    [HarmonyPatch(typeof(ForestGiantAI), "EatPlayerAnimation")]
    [HarmonyPrefix]
    private static void ForestKeeper(PlayerControllerB playerBeingEaten) {
        if(playerBeingEaten == KillPatch.Player) {
            // it's pretty common to escape the animation, so don't hard-claim death
            // TODO: try to hook into these animations a little more closely to claim death only when about to die
            KillPatch.Kill(ExtendedCauseOfDeath.Devoured, "Forest Keeper", 6);
        }
    }

    [HarmonyPatch(typeof(JesterAI), "killPlayerAnimation")]
    [HarmonyPrefix]
    private static void Jester(int playerId) {
        if(0 <= playerId && playerId < StartOfRound.Instance.allPlayerScripts.Length && StartOfRound.Instance.allPlayerScripts[playerId] == KillPatch.Player) {
            KillPatch.Kill(ExtendedCauseOfDeath.Mauled, "Jester", -1);
        }
    }

    [HarmonyPatch(typeof(MaskedPlayerEnemy), "killAnimation")]
    [HarmonyPrefix]
    private static void Masked(MaskedPlayerEnemy __instance) {
        if(__instance.inSpecialAnimationWithPlayer == KillPatch.Player) {
            // see comment on giant
            KillPatch.Kill(ExtendedCauseOfDeath.Infected, __instance.mimickingPlayer?.playerUsername ?? "Masked", 5);
        }
    }

    [HarmonyPatch(typeof(RadMechAI), "TorchPlayerAnimation")]
    [HarmonyPrefix]
    private static void OldBird(RadMechAI __instance) {
        if(__instance.inSpecialAnimationWithPlayer == KillPatch.Player) {
            // see comment on giant
            KillPatch.Kill(ExtendedCauseOfDeath.Incinerated, "Old Bird", 7);
        }
    }
}
