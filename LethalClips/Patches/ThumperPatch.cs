using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = CrawlerAI;


[HarmonyPatch(typeof(P), nameof(P.OnCollideWithPlayer))]
internal class ThumperPatch {
    private static void Prefix(
        Collider other
    ) {
        var player = other.GetComponent<PlayerControllerB>();
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Mauled;
        death.source = "Thumper";
    }
}
