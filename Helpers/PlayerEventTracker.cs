using Lacrimosum.ItemScripts;

namespace Lacrimosum.Helpers;

internal static class PlayerEventTracker
{
    public static void Init()
    {
        On.GameNetcodeStuff.PlayerControllerB.SpawnDeadBody += PlayerCorpseSpawned;
        RoR2Plugin.ModConsole.LogDebug("PlayerEventTracker: PlayerControllerB.SpawnDeadBody hooked!");
        On.GameNetcodeStuff.PlayerControllerB.DamagePlayer += PlayerDamaged;
        RoR2Plugin.ModConsole.LogDebug("PlayerEventTracker: PlayerControllerB.DamagePlayer hooked!");
        On.GameNetcodeStuff.PlayerControllerB.KillPlayer += BeforePlayerDeath;
        RoR2Plugin.ModConsole.LogDebug("PlayerEventTracker: PlayerControllerB.KillPlayerServerRpc hooked!");
    }

    private static void BeforePlayerDeath(On.GameNetcodeStuff.PlayerControllerB.orig_KillPlayer origKillPlayer, PlayerControllerB self, Vector3 vector3, bool spawnBody, CauseOfDeath ofDeath, int causeOfDeath, Vector3 positionOffset1)
    {
        RoR2Plugin.ModConsole.LogDebug($"{self.playerUsername} is scheduled to perish");
        var shouldRevive = false;
        foreach (var extraLife in DiosBestFriend.ActiveExtraLives)
        {
            shouldRevive = extraLife.CheckPlayerDeath(self);
        }

        if (shouldRevive)
        {
            HandleDiosBestFriend(self);
            RoR2Plugin.ModConsole.LogDebug($"HandleDiosBestFriend for {self.playerUsername}");
        }
        else
        {
            origKillPlayer(self, vector3, spawnBody, ofDeath, causeOfDeath, positionOffset1);
            RoR2Plugin.ModConsole.LogDebug($"{self.playerUsername} should die as they are not holding Dios' Best Friend");
        }
    }

    private static void PlayerDamaged(On.GameNetcodeStuff.PlayerControllerB.orig_DamagePlayer orig, PlayerControllerB self, int damageNumber, bool hasDamageSfx, bool callRPC, CauseOfDeath causeOfDeath, int deathAnimation, bool fallDamage, Vector3 force)
    {
        RoR2Plugin.ModConsole.LogDebug($"{self.playerUsername} damaged");
        orig(self, damageNumber, hasDamageSfx, callRPC, causeOfDeath, deathAnimation, fallDamage, force);
        
        HandlePennies(self);
        RoR2Plugin.ModConsole.LogDebug($"HandlePennies for {self.playerUsername}");
    }
    
    private static void PlayerCorpseSpawned(On.GameNetcodeStuff.PlayerControllerB.orig_SpawnDeadBody orig, PlayerControllerB self, int playerId, Vector3 bodyVelocity, int causeOfDeath, PlayerControllerB deadPlayerController, int deathAnimation, Transform overridePosition, Vector3 positionOffset)
    {
        RoR2Plugin.ModConsole.LogDebug($"{self.playerUsername}'s corpse got spawned");
        orig(self, playerId, bodyVelocity, causeOfDeath, deadPlayerController, deathAnimation, overridePosition, positionOffset);

        HandleWillOWisp(self);
        RoR2Plugin.ModConsole.LogDebug($"HandleWillOWisp for {self.playerUsername}");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static void HandleDiosBestFriend(PlayerControllerB player)
    {
        foreach (var extraLife in DiosBestFriend.ActiveExtraLives)
        {
            extraLife.PreventPlayerDeath(player);
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