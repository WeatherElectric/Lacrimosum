using Lacrimosum.ItemScripts;

namespace Lacrimosum.Helpers;

internal static class PlayerEventTracker
{
    public static void Init()
    {
        On.GameNetcodeStuff.PlayerControllerB.SpawnDeadBody += OnPlayerLateDeath;
        RoR2Plugin.ModConsole.LogDebug("PlayerControllerB.SpawnDeadBody hooked!");
        On.GameNetcodeStuff.PlayerControllerB.DamagePlayer += OnPlayerDamaged;
        RoR2Plugin.ModConsole.LogDebug("PlayerControllerB.DamagePlayer hooked!");
        On.GameNetcodeStuff.PlayerControllerB.KillPlayer += OnPlayerDeath;
        RoR2Plugin.ModConsole.LogDebug("PlayerControllerB.KillPlayerServerRpc hooked!");
    }

    private static void OnPlayerDeath(On.GameNetcodeStuff.PlayerControllerB.orig_KillPlayer origKillPlayer, PlayerControllerB self, Vector3 vector3, bool spawnBody, CauseOfDeath ofDeath, int causeOfDeath, Vector3 positionOffset1)
    {
        var shouldRevive = false;
        foreach (var extraLife in DiosBestFriend.ActiveExtraLives)
        {
            shouldRevive = extraLife.CheckPlayerDeath(self);
        }

        if (shouldRevive)
        {
            HandleDiosBestFriend(self);
        }
        else
        {
            origKillPlayer(self, vector3, spawnBody, ofDeath, causeOfDeath, positionOffset1);
        }
    }

    private static void OnPlayerDamaged(On.GameNetcodeStuff.PlayerControllerB.orig_DamagePlayer orig, PlayerControllerB self, int damageNumber, bool hasDamageSfx, bool callRPC, CauseOfDeath causeOfDeath, int deathAnimation, bool fallDamage, Vector3 force)
    {
        orig(self, damageNumber, hasDamageSfx, callRPC, causeOfDeath, deathAnimation, fallDamage, force);
        
        HandlePennies(self);
        RoR2Plugin.ModConsole.LogDebug("Player damaged!");
    }
    
    private static void OnPlayerLateDeath(On.GameNetcodeStuff.PlayerControllerB.orig_SpawnDeadBody orig, PlayerControllerB self, int playerId, Vector3 bodyVelocity, int causeOfDeath, PlayerControllerB deadPlayerController, int deathAnimation, Transform overridePosition, Vector3 positionOffset)
    {
        orig(self, playerId, bodyVelocity, causeOfDeath, deadPlayerController, deathAnimation, overridePosition, positionOffset);

        HandleWillOWisp(self);
        RoR2Plugin.ModConsole.LogDebug("Player late death!");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static void HandleDiosBestFriend(PlayerControllerB player)
    {
        foreach (var extraLife in DiosBestFriend.ActiveExtraLives)
        {
            extraLife.TeleportPlayer(player);
        }
    }

    private static void HandleWillOWisp(PlayerControllerB player)
    {
        foreach (var wisp in WilloWisp.ActiveWisps)
        {
            wisp.CheckPlayerDeath(player);
        }
    }
    
    private static void HandlePennies(PlayerControllerB player)
    {
        foreach (var penny in RollOfPennies.ActivePennies)
        {
            penny.CheckDamagedPlayer(player);
        }
    }
}