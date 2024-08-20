// ReSharper disable Unity.PreferAddressByIdToGraphicsParams
namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Dios' Best Friend")]
public class DiosBestFriend : ItemBehaviour
{
    public static readonly List<DiosBestFriend> ActiveExtraLives = [];
    public PlayerControllerB targetPlayer;
    public PlayerControllerB playerToRevive;

    public override void GrabItem()
    {
        base.GrabItem();
        targetPlayer = playerHeldBy;
        ActiveExtraLives.Add(this);
    }

    public bool CheckPlayerDeath(PlayerControllerB player)
    {
        return player == targetPlayer;
    }
    
    public void TeleportPlayer(PlayerControllerB player)
    {
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
        RevivePlayer();
        StartCoroutine(Despawn());
    }

    private void RevivePlayer()
    {
        var player = playerToRevive;
        player.isInElevator = true;
        player.isInHangarShipRoom = true;
        player.isInsideFactory = false;
        player.parentedToElevatorLastFrame = false;
        StartOfRound.Instance.SetPlayerObjectExtrapolate(false);
        player.TeleportPlayer(player.GetSpawnPosition());
        player.criticallyInjured = false;
        if (player.playerBodyAnimator != null) player.playerBodyAnimator.SetBool("Limp", false);
        player.bleedingHeavily = false;
        player.health = 100;
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
        if (!LastPlayerHeldBy.isPlayerDead) ActiveExtraLives.Remove(this);
        if (!LastPlayerHeldBy.isPlayerDead) targetPlayer = null;
    }
}