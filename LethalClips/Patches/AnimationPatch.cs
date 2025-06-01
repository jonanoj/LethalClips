using GameNetcodeStuff;
using HarmonyLib;

namespace LethalClips.Patches;


[HarmonyPatch]
public static class AnimationPatch {
    [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.killAnimation))]
    [HarmonyPrefix]
    public static void Bracken(FlowermanAI __instance) {
        var player = PlayerState.Of(__instance.inSpecialAnimationWithPlayer);
        player.Kill(ExtendedCauseOfDeath.Strangled, "Bracken", -1);
    }

    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.KillPlayer))]
    [HarmonyPrefix]
    public static void EyelessDog(int playerId) {
        var player = PlayerState.Of(Player.FromID(playerId));
        player.Kill(ExtendedCauseOfDeath.Mauled, "Eyeless Dog", -1);
    }

    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.EatPlayerAnimation))]
    [HarmonyPrefix]
    public static void ForestKeeper(PlayerControllerB playerBeingEaten) {
        // it's pretty common to escape the animation, so don't hard-claim death
        // TODO: try to hook into these animations a little more closely to claim death only when about to die
        var player = PlayerState.Of(playerBeingEaten);
        player.Kill(ExtendedCauseOfDeath.Devoured, "Forest Keeper", 6);
    }

    [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.killPlayerAnimation))]
    [HarmonyPrefix]
    public static void Jester(int playerId) {
        var player = PlayerState.Of(Player.FromID(playerId));
        player.Kill(ExtendedCauseOfDeath.Mauled, "Jester", -1);
    }

    [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.killAnimation))]
    [HarmonyPrefix]
    public static void Masked(MaskedPlayerEnemy __instance) {
        // see comment on giant
        var player = PlayerState.Of(__instance.inSpecialAnimationWithPlayer);
        player.Kill(ExtendedCauseOfDeath.Infected, __instance.mimickingPlayer?.playerUsername ?? "Masked", 5);
    }

    [HarmonyPatch(typeof(RadMechAI), nameof(RadMechAI.TorchPlayerAnimation))]
    [HarmonyPrefix]
    public static void OldBird(RadMechAI __instance) {
        // see comment on giant
        var player = PlayerState.Of(__instance.inSpecialAnimationWithPlayer);
        player.Kill(ExtendedCauseOfDeath.Incinerated, "Old Bird", 7);
    }
}
