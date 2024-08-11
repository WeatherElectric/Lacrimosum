namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Dios' Best Friend")]
public class DiosBestFriend : ItemBehaviour
{
    public static readonly List<DiosBestFriend> ActiveExtraLives = [];
    public PlayerControllerB playerToRevive;

    public override void GrabItem()
    {
        base.GrabItem();
        ActiveExtraLives.Add(this);
    }
    
    public void CheckPlayerDeath(PlayerControllerB player)
    {
        if (playerHeldBy == null) return;
        if (player != playerHeldBy) return;

        playerToRevive = player;
        ReviveServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ReviveServerRpc()
    {
        ReviveClientRpc();
    }

    [ClientRpc]
    public void ReviveClientRpc()
    {
        playerToRevive.Revive();
        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        playerHeldBy.DropAllHeldItems();
        yield return new WaitForSeconds(1f);
        NetworkObject.Despawn();
    }
    
    public override void DiscardItem()
    {
        base.DiscardItem();
        ActiveExtraLives.Remove(this);
    }
}