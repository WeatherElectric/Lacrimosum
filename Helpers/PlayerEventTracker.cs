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
        StartOfRound.Instance.StartCoroutine(HandleDiosBestFriend(self));
        RoR2Plugin.ModConsole.LogDebug("Player late death!");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static IEnumerator HandleDiosBestFriend(PlayerControllerB player)
    {
        // if the last player dies, the ship leaves before the 15 second timer is up, so just respawn instantly despite the UI issue, UI issue will be fixed upon leaving anyways
        if (StartOfRound.Instance.livingPlayers == 1)
        {
            yield return new WaitForSeconds(2f);
            foreach (var extraLife in DiosBestFriend.ActiveExtraLives)
            {
                extraLife.CheckPlayerDeath(player);
            }
        }
        else
        {
            yield return new WaitForSeconds(15f);
            foreach (var extraLife in DiosBestFriend.ActiveExtraLives)
            {
                extraLife.CheckPlayerDeath(player);
            }
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