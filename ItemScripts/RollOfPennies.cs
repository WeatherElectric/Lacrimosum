namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Roll Of Pennies")]
public class RollOfPennies : ItemBehaviour
{
    public static readonly List<RollOfPennies> ActivePennies = [];
    
    [Space(10f)]
    [Header("Roll Of Pennies Settings")]
    [Tooltip("The amount of credits to add to the item's value upon taking damage.")]
    public int addedCredits = 10;
    [Tooltip("The cap for the item's value.")]
    public int maxCredits = 100;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        addedCredits = RoR2Plugin.ModConfig.RollOfPenniesValueIncrease;
        maxCredits = RoR2Plugin.ModConfig.RollOfPenniesMaxValue;
    }
    
    public override void GrabItem()
    {
        base.GrabItem();
        ActivePennies.Add(this);
    }
    
    public void CheckDamagedPlayer(PlayerControllerB player)
    {
        if (player != playerHeldBy) return;
        if (scrapValue >= maxCredits) return;
        AddCreditsServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void AddCreditsServerRpc()
    {
        AddCreditsClientRpc();
    }
    
    [ClientRpc]
    private void AddCreditsClientRpc()
    {
        scrapValue += addedCredits;
        ScanNodeProperties.scrapValue = scrapValue;
        ScanNodeProperties.subText = $"Value: ${scrapValue}";
    }
    
    public override void DiscardItem()
    {
        base.DiscardItem();
        ActivePennies.Remove(this);
    }

    public override void OnNetworkDespawn()
    {
        ActivePennies.Remove(this);
        base.OnNetworkDespawn();
    }

    public override void OnDestroy()
    {
        ActivePennies.Remove(this);
        base.OnDestroy();
    }
}