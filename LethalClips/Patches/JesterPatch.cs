using HarmonyLib;

namespace LethalClips.Patches;

using P = JesterAI;


[HarmonyPatch(typeof(P), "killPlayerAnimation")]
internal class JesterPatch {
    private static void Prefix(
        int playerId
    ) {
        var player = StartOfRound.Instance.allPlayerScripts[playerId];
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Mauled;
        death.source = "Jester";
    }
}
