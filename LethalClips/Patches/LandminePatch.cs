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
        P __instance
    ) {
        var player = GameNetworkManager.Instance.localPlayerController;
        var death = State<Death>.Of(player);

        var blame = State<Player>.Of(__instance).player;
        death.cause = TranslatedCauseOfDeath.Exploded;
        death.source = blame == null || blame == player ? "Landmine" : blame.playerUsername;
    }
}
