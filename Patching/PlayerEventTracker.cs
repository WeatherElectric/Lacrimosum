using Lacrimosum.Assets;
using Lacrimosum.ItemScripts;

namespace Lacrimosum.Patching;

internal static class PlayerEventTracker
{
    public static void Init()
    {
        On.GameNetcodeStuff.PlayerControllerB.SpawnDeadBody += PlayerCorpseSpawned;
        RoR2Plugin.ModConsole.LogDebug("PlayerEventTracker: PlayerControllerB.SpawnDeadBody hooked!");
        On.GameNetcodeStuff.PlayerControllerB.DamagePlayer += PlayerDamaged;
        RoR2Plugin.ModConsole.LogDebug("PlayerEventTracker: PlayerControllerB.DamagePlayer hooked!");
        On.GameNetcodeStuff.PlayerControllerB.Start += PlayerStart;
        RoR2Plugin.ModConsole.LogDebug("PlayerEventTracker: PlayerControllerB.Start hooked!");
    }

    private static void PlayerDamaged(On.GameNetcodeStuff.PlayerControllerB.orig_DamagePlayer orig, PlayerControllerB self, int damageNumber, bool hasDamageSfx, bool callRPC, CauseOfDeath causeOfDeath, int deathAnimation, bool fallDamage, Vector3 force)
    {
        RoR2Plugin.ModConsole.LogDebug($"{self.playerUsername} damaged");

        var saferSpaces = SaferSpaces.ActiveSpaces;
        if (saferSpaces.Count == 0)
        {
            HandlePennies(self);
            RoR2Plugin.ModConsole.LogDebug($"HandlePennies for {self.playerUsername}");
            orig(self, damageNumber, hasDamageSfx, callRPC, causeOfDeath, deathAnimation, fallDamage, force);
            RoR2Plugin.ModConsole.LogDebug($"Player {self.playerUsername} will take damage because no one is holding a Safer Spaces.");
        }
        
        foreach (var saferSpace in saferSpaces)
        {
            if (!saferSpace.IsPlayerHolding(self)) continue;
            if (saferSpace.CanBlock)
            {
                RoR2Plugin.ModConsole.LogDebug($"Player {self.playerUsername} will not take damage because {saferSpace.gameObject.name} is holding a Safer Spaces.");
                saferSpace.PlaySound();
                saferSpace.StartCooldown();
            }
            else
            {
                HandlePennies(self);
                RoR2Plugin.ModConsole.LogDebug($"HandlePennies for {self.playerUsername}");
                orig(self, damageNumber, hasDamageSfx, callRPC, causeOfDeath, deathAnimation, fallDamage, force);
                RoR2Plugin.ModConsole.LogDebug($"Player {self.playerUsername} will take damage because their safer spaces is on cooldown or is out of battery.");
            }
        }
    }
    
    private static void PlayerCorpseSpawned(On.GameNetcodeStuff.PlayerControllerB.orig_SpawnDeadBody orig, PlayerControllerB self, int playerId, Vector3 bodyVelocity, int causeOfDeath, PlayerControllerB deadPlayerController, int deathAnimation, Transform overridePosition, Vector3 positionOffset)
    {
        RoR2Plugin.ModConsole.LogDebug($"{self.playerUsername}'s corpse got spawned");
        orig(self, playerId, bodyVelocity, causeOfDeath, deadPlayerController, deathAnimation, overridePosition, positionOffset);

        HandleWillOWisp(self);
        RoR2Plugin.ModConsole.LogDebug($"HandleWillOWisp for {self.playerUsername}");
    }
    
    private static void PlayerStart(On.GameNetcodeStuff.PlayerControllerB.orig_Start orig, PlayerControllerB self)
    {
        RoR2Plugin.ModConsole.LogDebug($"Player {self.playerUsername} started!");
        orig(self);
        
        SetupBungusWard(self);
        SetupSaferSpacesBubble(self);
    }

    private static void SetupBungusWard(PlayerControllerB player)
    {
        var bungusWard = Object.Instantiate(ModAssets.BungusMushroomWardPrefab, player.transform);
        bungusWard.name = "MushroomWard";
        bungusWard.SetActive(false);
        RoR2Plugin.ModConsole.LogDebug($"Set up BungusWard for {player.playerUsername}!");
    }

    private static void SetupSaferSpacesBubble(PlayerControllerB player)
    {
        var saferSpacesBubble = Object.Instantiate(ModAssets.SaferSpacesBubblePrefab, player.transform);
        saferSpacesBubble.name = "SaferSpacesBubble";
        saferSpacesBubble.SetActive(false);
        saferSpacesBubble.transform.localPosition = new Vector3(0, 1.25f, 0);
        RoR2Plugin.ModConsole.LogDebug($"Set up SaferSpacesBubble for {player.playerUsername}!");
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