namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Will-O'-The-Wisp")]
public class WilloWisp : ItemBehaviour
{
    public static readonly List<WilloWisp> ActiveWisps = [];
    
    [Space(10f)]
    [Header("Will-O'-The-Wisp Settings")]
    [Tooltip("The gameobject of the particles.")]
    public GameObject particles;

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
    
    public override void EquipItem()
    {
        base.EquipItem();
        particles.SetActive(true);
    }

    public override void PocketItem()
    {
        base.PocketItem();
        particles.SetActive(false);
    }
}