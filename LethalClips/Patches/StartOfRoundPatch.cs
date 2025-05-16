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

        if (!Plugin.ClipConfig.ClipRound.Value)
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

        if (!Plugin.ClipConfig.ClipRound.Value)
        {
            return;
        }

        try
        {
            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                "Ship has leff",
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