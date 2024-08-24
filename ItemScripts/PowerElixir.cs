namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Power Elixir")]
public class PowerElixir : ItemBehaviour
{
    public static readonly List<PowerElixir> ActiveElixirs = [];

    [Space(10f)] [Header("Power Elixir Settings")] 
    [Tooltip("The amount of health the player must reach to activate.")]
    public int healthThreshold = 25;
    [Tooltip("The percentage of health to heal when activated.")]
    public float healPercentage = 75f;
    [Tooltip("The sounds to play when activated.")]
    public AudioClip[] activationSounds;

    public override void OnNetworkSpawn()
    {
        healthThreshold = RoR2Plugin.ModConfig.PowerElixirHealthThreshold;
        healPercentage = RoR2Plugin.ModConfig.PowerElixirHealPercentage;
    }
    
    public override void GrabItem()
    {
        base.GrabItem();
        ActiveElixirs.Add(this);
    }
    
    public override void DiscardItem()
    {
        base.DiscardItem();
        ActiveElixirs.Remove(this);
    }

    public override void OnNetworkDespawn()
    {
        ActiveElixirs.Remove(this);
        base.OnNetworkDespawn();
    }
    
    public override void OnDestroy()
    {
        ActiveElixirs.Remove(this);
        base.OnDestroy();
    }
    
    public void CheckHealth(PlayerControllerB player, int damageNumber)
    {
        if (player != playerHeldBy) return;
        var healthAfterDamage = player.health - damageNumber;
        if (healthAfterDamage > healthThreshold) return;
        HealPlayerServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void HealPlayerServerRpc()
    {
        HealPlayerClientRpc();
    }
    
    [ClientRpc]
    private void HealPlayerClientRpc()
    {
        // calculate health and make sure it doesn't exceed 100
        var currHealth = playerHeldBy.health;
        var calculatedHeal = currHealth * (healPercentage / 100);
        if (calculatedHeal > 100) calculatedHeal = 100;
        var roundedHeal = calculatedHeal.RoundToInt();
        
        AudioSource.PlayOneShot(activationSounds[Random.Range(0, activationSounds.Length)]);
        
        // heal player and reset flags
        playerHeldBy.health = roundedHeal;
        playerHeldBy.criticallyInjured = false;
        if (playerHeldBy.playerBodyAnimator) playerHeldBy.playerBodyAnimator.SetBool("Limp", false);
        playerHeldBy.bleedingHeavily = false;

        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(1f);
        DiscardItemServerRpc();
        NetworkObject.Despawn();
    }
}