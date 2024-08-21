using Utilities = Lacrimosum.Helpers.Utilities;

namespace Lacrimosum.Patching;

internal static class MineReplacement
{
    public static void Init()
    {
        On.Landmine.SpawnExplosion += OnMineSpawnExplosion;
        RoR2Plugin.ModConsole.LogDebug("MineReplacement: Landmine.SpawnExplosion hooked!");
    }

    private static void OnMineSpawnExplosion(On.Landmine.orig_SpawnExplosion orig, Vector3 explosionPosition, bool spawnExplosionEffect, float killRange, float damageRange, int nonLethalDamage, float physicsForce, GameObject overridePrefab, bool goThroughCar)
    {
        if (RoR2Plugin.ModConfig.DontOverrideMineCode)
        {
            orig(explosionPosition, spawnExplosionEffect, killRange, damageRange, nonLethalDamage, physicsForce, overridePrefab, goThroughCar);
            return;
        }
        
        // Basegame mines just call KillPlayer. This explosion method damages them. I need them to only damage the player so that Safer Spaces works.
        Utilities.CreateExplosion(explosionPosition, spawnExplosionEffect, 20, killRange, damageRange, physicsForce.RoundToInt());
    }
}