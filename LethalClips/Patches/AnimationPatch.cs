using GameNetcodeStuff;
using HarmonyLib;

namespace LethalClips.Patches;


[HarmonyPatch]
public static class AnimationPatch {
    [HarmonyPatch(typeof(FlowermanAI), "killAnimation")]
    [HarmonyPrefix]
    public static void Bracken(FlowermanAI __instance) {
        var player = KillState.Of(__instance.inSpecialAnimationWithPlayer);
        player.Kill(ExtendedCauseOfDeath.Strangled, "Bracken", -1);
    }

    [HarmonyPatch(typeof(MouthDogAI), "KillPlayer")]
    [HarmonyPrefix]
    public static void EyelessDog(int playerID) {
        if(0 <= playerID && playerID < StartOfRound.Instance.allPlayerScripts.Length) {
            var player = KillState.Of(StartOfRound.Instance.allPlayerScripts[playerID]);
            player.Kill(ExtendedCauseOfDeath.Mauled, "Eyeless Dog", -1);
        }
    }

    [HarmonyPatch(typeof(ForestGiantAI), "EatPlayerAnimation")]
    [HarmonyPrefix]
    public static void ForestKeeper(PlayerControllerB playerBeingEaten) {
        // it's pretty common to escape the animation, so don't hard-claim death
        // TODO: try to hook into these animations a little more closely to claim death only when about to die
        var player = KillState.Of(playerBeingEaten);
        player.Kill(ExtendedCauseOfDeath.Devoured, "Forest Keeper", 6);
    }

    [HarmonyPatch(typeof(JesterAI), "killPlayerAnimation")]
    [HarmonyPrefix]
    public static void Jester(int playerId) {
        if(0 <= playerId && playerId < StartOfRound.Instance.allPlayerScripts.Length) {
            var player = KillState.Of(StartOfRound.Instance.allPlayerScripts[playerId]);
            player.Kill(ExtendedCauseOfDeath.Mauled, "Jester", -1);
        }
    }

    [HarmonyPatch(typeof(MaskedPlayerEnemy), "killAnimation")]
    [HarmonyPrefix]
    public static void Masked(MaskedPlayerEnemy __instance) {
        // see comment on giant
        var player = KillState.Of(__instance.inSpecialAnimationWithPlayer);
        player.Kill(ExtendedCauseOfDeath.Infected, __instance.mimickingPlayer?.playerUsername ?? "Masked", 5);
    }

    [HarmonyPatch(typeof(RadMechAI), "TorchPlayerAnimation")]
    [HarmonyPrefix]
    public static void OldBird(RadMechAI __instance) {
        // see comment on giant
        var player = KillState.Of(__instance.inSpecialAnimationWithPlayer);
        player.Kill(ExtendedCauseOfDeath.Incinerated, "Old Bird", 7);
    }
}
