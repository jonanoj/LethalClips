using HarmonyLib;

namespace LethalClips.Patches;

using P = MouthDogAI;


[HarmonyPatch(typeof(P), "KillPlayer")]
internal class DogPatch {
    private static void Prefix(
        int playerId,
        ref Death __state
    ) {
        var player = StartOfRound.Instance.allPlayerScripts[playerId];
        var death = State<Death>.Of(player);
        __state = new() {
            cause = death.cause,
            source = death.source
        };
        death.cause = TranslatedCauseOfDeath.Mauled;
        death.source = "Eyeless Dog";
    }

    private static void Postfix(
        int playerId,
        Death __state
    ) {
        // restore the previous cause of death, in case no kill happened
        var player = StartOfRound.Instance.allPlayerScripts[playerId];
        var death = State<Death>.Of(player);
        death.cause = __state.cause;
        death.source = __state.source;
    }
}
