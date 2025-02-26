using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = Landmine;


internal class Player {
    internal PlayerControllerB player;
}


[HarmonyPatch(typeof(P), nameof(P.Detonate))]
internal class LandminePatch_Detonate {
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

        var blame = State<Player>.Of(__instance).player;
        death.cause = TranslatedCauseOfDeath.Exploded;
        death.source = blame == null || blame == player ? "Landmine" : blame.playerUsername;
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
