using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(EnemyAI))]
public static class DamagePatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(EnemyAI.OnCollideWithPlayer))]
    private static void OnCollideWithPlayer(EnemyAI __instance, Collider other) {
        const int KILL = -1;
        var overrideIsInsideFactoryCheck = __instance is DressGirlAI;

        if(!__instance.MeetsStandardPlayerCollisionConditions(other, false, overrideIsInsideFactoryCheck)) {
            return;
        }

        var (cause, source, damage) = __instance switch {
            ClaySurgeonAI => (ExtendedCauseOfDeath.Snipped, "Barber", KILL),
            RedLocustBees => (ExtendedCauseOfDeath.Electrocuted, "Circuit Bees", 10),
            ButlerEnemyAI => (ExtendedCauseOfDeath.Stabbed, "Butler", 10),
            SpringManAI => (ExtendedCauseOfDeath.Springed, "Coil-Head", 90),
            BushWolfEnemy => (ExtendedCauseOfDeath.Mauled, "Kidnapper Fox", KILL),
            DressGirlAI => (ExtendedCauseOfDeath.Died, "", KILL),
            BaboonBirdAI => (ExtendedCauseOfDeath.Stabbed, "Baboon Hawk", 20),
            ButlerBeesEnemyAI => (ExtendedCauseOfDeath.Stabbed, "Mask Hornets", 10),
            PufferAI => (ExtendedCauseOfDeath.Embarrassing, "", 20),
            HoarderBugAI => (ExtendedCauseOfDeath.Mauled, "Hoarding Bug", 30),
            CaveDwellerAI => (ExtendedCauseOfDeath.Mauled, "Maneater", KILL),
            NutcrackerEnemyAI => (ExtendedCauseOfDeath.Kicked, "Nutcracker", KILL),
            BlobAI => (ExtendedCauseOfDeath.Disintegrated, "Hydrogere", 35),
            SandSpiderAI => (ExtendedCauseOfDeath.Mauled, "Bunker Spider", 90),
            CrawlerAI => (ExtendedCauseOfDeath.Mauled, "Thumper", 40),
            SandWormAI => (ExtendedCauseOfDeath.Devoured, "Earth Leviathan", KILL),
            _ => default
        };

        // TODO: get enemy internal timers to check if they can actually damage the player
        var player = KillState.Player;
        if(damage == KILL) {
            player.Kill(cause, source);
        } else {
            player.Damage(cause, source, damage);
        }
    }
}
