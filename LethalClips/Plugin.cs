using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace LethalClips;


[BepInPlugin(GUID, NAME, VERSION)]
[BepInDependency("com.lalabuff.lethal.steamapiupgrade")]
public class Plugin : BaseUnityPlugin {
    const string GUID = "com.lalabuff.lethal.lethalclips";
    const string NAME = "LethalClips";
    const string VERSION = "0.0.3";

    internal static ManualLogSource Log;
    internal static ClipConfig ClipConfig;


    internal void Awake()
    {
        Log = Logger;
        ClipConfig = new ClipConfig(base.Config);

        if (!ClipConfig.EnableMod.Value)
        {
            Log.LogWarning($"Mod is disabled in the config, not patching.");
            return;
        }

        Harmony harmony = new(GUID);
        harmony.PatchAll();
        foreach (var item in harmony.GetPatchedMethods())
        {
            Log.LogInfo($"Patched method {item}.");
        }
        Log.LogInfo($"Successfully loaded {NAME} ({GUID}) v{VERSION}!");
    }
}
