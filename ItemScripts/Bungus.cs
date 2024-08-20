using Lacrimosum.Assets;

namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Bustling Fungus")]
public class Bungus : ItemBehaviour
{
    [Space(10f)]
    [Header("Bustling Fungus Settings")]
    [Tooltip("The amount of health to heal per second when in the zone.")]
    public int addedHealth = 10;
    [Tooltip("The time between each heal.")]
    public float healInterval = 1f;
    [Tooltip("How long the player must be standing for the zone to activate.")]
    public float activationTime = 3f;
    [Tooltip("The sound to play when the zone activates.")]
    public AudioClip activationSound;
    
    private bool _isHealing;
    private GameObject _mushroomWard;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        addedHealth = RoR2Plugin.ModConfig.BungusHealthIncrease;
        healInterval = RoR2Plugin.ModConfig.BungusHealInterval;
        activationTime = RoR2Plugin.ModConfig.BungusActivationTime;
        if (RoR2Plugin.ModConfig.BungusMode) ScanNodeProperties.headerText = "Bungus";
    }
    
    public override void GrabItem()
    {
        base.GrabItem();
        _mushroomWard = playerHeldBy.GetMushroomWard();
    }

    public override void Update()
    {
        base.Update();
        if (!playerHeldBy) return;
        if (playerHeldBy.timeSincePlayerMoving > activationTime && !_isHealing)
        {
            StartHealServerRpc();
        }
        else if (playerHeldBy.timeSincePlayerMoving < activationTime && _isHealing)
        {
            StopHealServerRpc();
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void StartHealServerRpc()
    {
        StartHealClientRpc();
    }

    [ClientRpc]
    public void StartHealClientRpc()
    {
        _isHealing = true;
        AudioSource.PlayOneShot(activationSound);
        _mushroomWard.SetActive(true);
        StartCoroutine(HealRoutine());
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void StopHealServerRpc()
    {
        StopHealClientRpc();
    }
    
    [ClientRpc]
    public void StopHealClientRpc()
    {
        _isHealing = false;
        _mushroomWard.SetActive(false);
    }

    private IEnumerator HealRoutine()
    {
        while (_isHealing)
        {
            // ReSharper disable once Unity.PreferNonAllocApi
            var playersInZone = Physics.OverlapSphere(transform.position, 3f, LayerMask.GetMask("Player"));
            foreach (var player in playersInZone)
            {
                if (!player.TryGetComponent(out PlayerControllerB playerController)) continue;
                if (playerController.health != 100) playerController.health += addedHealth;
                yield return new WaitForSeconds(healInterval);
            }
        }
    }
    
    public override void DiscardItem()
    {
        base.DiscardItem();
        if (_isHealing) StopHealServerRpc();
        _mushroomWard.SetActive(false);
        _mushroomWard = null;
    }
}

internal static class BungusHelper
{
    public static void Init()
    {
        On.GameNetcodeStuff.PlayerControllerB.Start += OnPlayerStart;
        RoR2Plugin.ModConsole.LogDebug("BungusHelper: PlayerControllerB.Start hooked!");
    }

    private static void OnPlayerStart(On.GameNetcodeStuff.PlayerControllerB.orig_Start orig, PlayerControllerB self)
    {
        RoR2Plugin.ModConsole.LogDebug($"Player {self.playerUsername} started!");
        orig(self);
        var bungusWard = Object.Instantiate(ModAssets.BungusMushroomWardPrefab, self.transform);
        bungusWard.name = "MushroomWard";
        bungusWard.SetActive(false);
        RoR2Plugin.ModConsole.LogDebug($"Set up BungusWard for {self.playerUsername}!");
    }
}