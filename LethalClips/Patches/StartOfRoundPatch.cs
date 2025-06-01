using HarmonyLib;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(StartOfRound))]
public class StartOfRoundPatch {
    [HarmonyPatch(nameof(StartOfRound.openingDoorsSequence))]
    [HarmonyPrefix]
    public static void OpeningDoorsSequence() {
        // TODO: make this a game phase
        if(Config.Clips.Rounds.Value) {
            Steam.AddEvent("Round start", "The ship has landed", Steam.Icon.Flag);
        }
    }

    [HarmonyPatch(nameof(StartOfRound.ShipHasLeft))]
    [HarmonyPrefix]
    public static void ShipHasLeft() {
        if(Config.Clips.Rounds.Value) {
            Steam.AddEvent("Round end", "The ship has left", Steam.Icon.Completed);
        }
    }
}
