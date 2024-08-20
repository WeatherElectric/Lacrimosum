namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Bison Steak")]
public class BisonSteak : ItemBehaviour
{
    [Space(10f)]
    [Header("Bison Steak Settings")]
    [Tooltip("The amount of health to add when held.")]
    public int addedHealth = 25;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        addedHealth = RoR2Plugin.ModConfig.BisonSteakHealthIncrease;
    }
    
    public override void GrabItem()
    {
        base.GrabItem();
        if (IsOwner) AddHealthServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void AddHealthServerRpc()
    {
        AddHealthClientRpc();
    }
    
    [ClientRpc]
    private void AddHealthClientRpc()
    {
        playerHeldBy.health += addedHealth;
    }
    
    public override void DiscardItem()
    {
        base.DiscardItem();
        if (IsOwner) RemoveHealthServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RemoveHealthServerRpc()
    {
        RemoveHealthClientRpc();
    }
    
    [ClientRpc]
    private void RemoveHealthClientRpc()
    {
        LastPlayerHeldBy.health -= addedHealth;
    }
}