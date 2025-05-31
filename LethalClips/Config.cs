using BepInEx.Configuration;

namespace LethalClips;


public static class Config {
    public class ConfigGroup(ConfigFile config, string group) {
        public ConfigEntry<T> Bind<T>(string key, T defaultValue, string description, AcceptableValueBase acceptableValues = null, params object[] tags) {
            return config.Bind(group, key, defaultValue, new ConfigDescription(description, acceptableValues, tags));
        }
    }

    public static class General {
        public static ConfigEntry<bool> Enabled { get; private set; }

        public static void Initialize(ConfigGroup config) {
            Enabled = config.Bind("Enabled", true, "Whether the mod should be loaded on game startup");
        }
    }

    public static class Clips {
        public static ConfigEntry<bool> Deaths { get; private set; }
        public static ConfigEntry<bool> Apparatus { get; private set; }
        public static ConfigEntry<bool> Teleporter { get; private set; }
        public static ConfigEntry<bool> Rounds { get; private set; }

        public static void Initialize(ConfigGroup config) {
            Deaths = config.Bind("Clip Deaths", true, "Create clip markers for player deaths");
            Apparatus = config.Bind("Clip Apparatus", true, "Create clip markers when someone takes the apparatus");
            Teleporter = config.Bind("Clip Teleporter", true, "Create clips markers when you are teleported by one of the teleporters");
            Rounds = config.Bind("Clip Round", true, "Create clip markers when the round begins and ends");
        }
    }

    public static void Initialize(ConfigFile config) {
        General.Initialize(new ConfigGroup(config, "General"));
        Clips.Initialize(new ConfigGroup(config, "Clips"));
    }
}
