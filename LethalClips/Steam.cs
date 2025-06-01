using Steamworks;
using Steamworks.Data;
using System;

namespace LethalClips;


public static class Steam {
    public enum Icon {
        Death,
        Caution,
        Transfer,
        Flag,
        Completed,
    }

    public static string IconToString(Icon icon) {
        return icon switch {
            Icon.Death => "steam_death",
            Icon.Caution => "steam_caution",
            Icon.Transfer => "steam_transfer",
            Icon.Flag => "steam_flag",
            Icon.Completed => "steam_completed",
            _ => "steam_marker"
        };
    }

    public static TimelineEventHandle? AddEvent(string title, string description, Icon icon, uint priority = 0, float offset = 0, TimelineEventClipPriority possibleClip = TimelineEventClipPriority.Standard) {
        try {
            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(title, description, IconToString(icon), priority, offset, possibleClip);
            Plugin.Log.LogInfo($"Added timeline event {timelineEvent}.");
            return timelineEvent;
        } catch(Exception e) {
            Plugin.Log.LogError($"Failed to add timeline event '{title}': {e}");
            return null;
        }
    }
}
