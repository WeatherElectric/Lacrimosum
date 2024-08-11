using Utilities = Lacrimosum.Helpers.Utilities;

namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Will-O'-The-Wisp")]
public class WilloWisp : ItemBehaviour
{
    public static readonly List<WilloWisp> ActiveWisps = [];

    public override void GrabItem()
    {
        base.GrabItem();
        ActiveWisps.Add(this);
    }
    
    public void CheckPlayerDeath(PlayerControllerB player)
    {
        if (playerHeldBy == null) return;
        if (player != playerHeldBy) return;
        
        ExplodeServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ExplodeServerRpc()
    {
        ExplodeClientRpc();
    }

    [ClientRpc]
    public void ExplodeClientRpc()
    {
        StartCoroutine(Explode());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    protected virtual IEnumerator Explode()
    {
        Utilities.CreateExplosion(transform.position, true, 100, 0f, 6.4f);
        yield return new WaitForSeconds(2f);
        if (IsServer) gameObject.GetComponent<NetworkObject>().Despawn();
    }
    
    public override void DiscardItem()
    {
        base.DiscardItem();
        ActiveWisps.Remove(this);
    }
}