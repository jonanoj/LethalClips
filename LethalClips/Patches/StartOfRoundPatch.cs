using System;
using HarmonyLib;
using LethalClips;
using Steamworks;


[HarmonyPatch(typeof(StartOfRound))]
public class StartOfRoundPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(StartOfRound.openingDoorsSequence))]
    public static void OpeningDoorsSequence() {
        if(Config.Clips.Rounds.Value) {
            Steam.AddEvent("Round start", "The ship has landed", Steam.Icon.Flag);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(StartOfRound.ShipHasLeft))]
    public static void ShipHasLeft() {
        if(Config.Clips.Rounds.Value) {
            Steam.AddEvent("Round end", "The ship has left", Steam.Icon.Completed);
        }
    }
}
