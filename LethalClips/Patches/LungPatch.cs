using System;
using HarmonyLib;
using LethalClips;
using Steamworks;

// Lung AKA Apparatus
[HarmonyPatch(typeof(LungProp), "DisconnectFromMachinery")]
internal class LungPatch
{
    private static void Prefix()
    {
        Plugin.Log.LogDebug("Someone took the apparatus");
        try
        {
            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                "WARNING",
                "Someone took the apparatus",
                "steam_caution",
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