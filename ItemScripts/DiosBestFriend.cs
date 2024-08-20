// ReSharper disable Unity.PreferAddressByIdToGraphicsParams
namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Dios' Best Friend")]
public class DiosBestFriend : ItemBehaviour
{
    public static readonly List<DiosBestFriend> ActiveExtraLives = [];

    public override void GrabItem()
    {
        base.GrabItem();
        ActiveExtraLives.Add(this);
    }

    public bool CheckPlayerDeath(PlayerControllerB player)
    {
        return player == playerHeldBy;
    }
    
    public void PreventPlayerDeath(PlayerControllerB player)
    {
        if (player != playerHeldBy) return; // just in case
        TeleportServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void TeleportServerRpc()
    {
        TeleportClientRpc();
    }

    [ClientRpc]
    public void TeleportClientRpc()
    {
        TeleportPlayer();
        StartCoroutine(Despawn());
    }

    private void TeleportPlayer()
    {
        playerHeldBy.isFallingNoJump = false;
        playerHeldBy.isFallingFromJump = false;
        playerHeldBy.fallValue = 0;
        playerHeldBy.isInElevator = true;
        playerHeldBy.isInHangarShipRoom = true;
        playerHeldBy.isInsideFactory = false;
        playerHeldBy.parentedToElevatorLastFrame = false;
        playerHeldBy.TeleportPlayer(playerHeldBy.GetSpawnPosition());
        playerHeldBy.criticallyInjured = false;
        if (playerHeldBy.playerBodyAnimator != null) playerHeldBy.playerBodyAnimator.SetBool("Limp", false);
        playerHeldBy.bleedingHeavily = false;
        playerHeldBy.health = 100;
    }

    private IEnumerator Despawn()
    {
        playerHeldBy.DropAllHeldItemsAndSync();
        yield return new WaitForSeconds(1f);
        NetworkObject.Despawn();
    }

    public override void DiscardItem()
    {
        base.DiscardItem();
        if (!LastPlayerHeldBy.isPlayerDead) ActiveExtraLives.Remove(this);
    }
}