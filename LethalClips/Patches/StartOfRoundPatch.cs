using System;
using HarmonyLib;
using LethalClips;
using Steamworks;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRoundPatch
{
    [HarmonyPatch(nameof(StartOfRound.openingDoorsSequence))]
    [HarmonyPrefix]
    private static void OpeningDoorsSequence()
    {
        Plugin.Log.LogDebug("OpeningDoorsSequence called");

        if (!Config.Clips.Rounds.Value)
        {
            return;
        }

        // TODO: consider using the SetTimelineGameMode API instead
        try
        {
            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                "Ship has landed",
                "Round start",
                "steam_flag",
                0,
                0,
                TimelineEventClipPriority.Standard
            );
            Plugin.Log.LogDebug($"Added timeline event {timelineEvent}.");
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"Failed to add timeline event: {e}");
        }
    }

    [HarmonyPatch(nameof(StartOfRound.ShipHasLeft))]
    [HarmonyPrefix]
    private static void ShipHasLeft()
    {
        Plugin.Log.LogDebug("ShipHasLeft called");

        if (!Config.Clips.Rounds.Value)
        {
            return;
        }

        try
        {
            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                "Ship has left",
                "Round ended",
                "steam_completed",
                0,
                0,
                TimelineEventClipPriority.Standard
            );
            Plugin.Log.LogDebug($"Added timeline event {timelineEvent}.");
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"Failed to add timeline event: {e}");
        }
    }
}