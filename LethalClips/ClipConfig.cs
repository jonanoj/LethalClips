using BepInEx.Configuration;

class ClipConfig
{
    // General
    public readonly ConfigEntry<bool> EnableMod; // useful when installed as part of a modpack

    // Clips
    public readonly ConfigEntry<bool> ClipDeaths;
    public readonly ConfigEntry<bool> ClipApparatus;
    public readonly ConfigEntry<bool> ClipTeleporter;
    public readonly ConfigEntry<bool> ClipRound;

    public ClipConfig(ConfigFile cfg)
    {
        cfg.SaveOnConfigSet = false;

        EnableMod = cfg.Bind("General", "Enabled", true, "Set to false to disable the mod entirely.");
        cfg.Save();

        ClipDeaths = cfg.Bind("Clips", "Clip Deaths", true, "Create clips for player deaths");
        ClipApparatus = cfg.Bind("Clips", "Clip Apparatus", true, "Create clips when someone takes the apparatus");
        ClipTeleporter = cfg.Bind("Clips", "Clip Teleporter", true, "Create clips when you are teleported by one of the teleporters");
        ClipRound = cfg.Bind("Clips", "Clip Round", true, "Create clips when the round begins and ends");

        cfg.Save();
        cfg.SaveOnConfigSet = true;
    }
}