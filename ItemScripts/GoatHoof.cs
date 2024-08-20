namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Paul's Goat Hoof")]
public class GoatHoof : ItemBehaviour
{
    [Space(10f)] 
    [Header("Paul's Goat Hoof Settings")] 
    [Tooltip("The amount of speed to add when held.")]
    public float addedSpeedPercent = 4.6f;
    
    public override void OnNetworkSpawn()
    {
        addedSpeedPercent = RoR2Plugin.Config.GoatHoofSpeedIncrease;
    }
    
    public override void GrabItem()
    {
        base.GrabItem();
        if (IsOwner) AddSpeedServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void AddSpeedServerRpc()
    {
        AddSpeedClientRpc();
    }
    
    [ClientRpc]
    private void AddSpeedClientRpc()
    {
        playerHeldBy.movementSpeed += addedSpeedPercent;
    }
    
    public override void DiscardItem()
    {
        base.DiscardItem();
        if (IsOwner) RemoveSpeedServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RemoveSpeedServerRpc()
    {
        RemoveSpeedClientRpc();
    }
    
    [ClientRpc]
    private void RemoveSpeedClientRpc()
    {
        LastPlayerHeldBy.movementSpeed -= addedSpeedPercent;
    }
}