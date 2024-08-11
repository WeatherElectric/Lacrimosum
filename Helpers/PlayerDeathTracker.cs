using Lacrimosum.ItemScripts;

namespace Lacrimosum.Helpers;

internal static class PlayerDeathTracker
{
    public static void Init()
    {
        On.GameNetcodeStuff.PlayerControllerB.SpawnDeadBody += OnPlayerLateDeath;
        RoR2Plugin.ModConsole.LogDebug("PlayerControllerB.SpawnDeadBody hooked!");
        On.GameNetcodeStuff.PlayerControllerB.KillPlayer += OnPlayerDeathEarly;
        RoR2Plugin.ModConsole.LogDebug("PlayerControllerB.KillPlayer hooked!");
    }

    private static void OnPlayerDeathEarly(On.GameNetcodeStuff.PlayerControllerB.orig_KillPlayer orig, PlayerControllerB self, Vector3 bodyVelocity, bool spawnBody, CauseOfDeath causeOfDeath, int deathAnimation, Vector3 positionOffset)
    {
        orig(self, bodyVelocity, spawnBody, causeOfDeath, deathAnimation, positionOffset);
        
        
        RoR2Plugin.ModConsole.LogDebug("Player death early!");
    }

    private static void OnPlayerLateDeath(On.GameNetcodeStuff.PlayerControllerB.orig_SpawnDeadBody orig, PlayerControllerB self, int playerId, Vector3 bodyVelocity, int causeOfDeath, PlayerControllerB deadPlayerController, int deathAnimation, Transform overridePosition, Vector3 positionOffset)
    {
        orig(self, playerId, bodyVelocity, causeOfDeath, deadPlayerController, deathAnimation, overridePosition, positionOffset);

        HandleWillOWisp(self);
        HandleDiosBestFriend(self);
        RoR2Plugin.ModConsole.LogDebug("Player late death!");
    }

    private static void HandleWillOWisp(PlayerControllerB player)
    {
        foreach (var wisp in WilloWisp.ActiveWisps)
        {
            wisp.CheckPlayerDeath(player);
        }
    }

    private static void HandleDiosBestFriend(PlayerControllerB player)
    {
        foreach (var extraLife in DiosBestFriend.ActiveExtraLives)
        {
            extraLife.CheckPlayerDeath(player);
        }
    }
}