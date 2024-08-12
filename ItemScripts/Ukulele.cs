namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Ukulele")]
[RequireComponent(typeof(Shovel))]
public class Ukulele : NetworkBehaviour
{
    public static readonly List<Ukulele> ActiveUkuleles = [];
    
    [Header("Ukulele Settings")]
    [Tooltip("The particles to play when the ukulele hits something")]
    public ParticleSystem zapEffect;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ActiveUkuleles.Add(this);
    }
    
    private Shovel Shovel => GetComponent<Shovel>();
    
    public void HandleShovelHit(Shovel shovel)
    {
        if (shovel != Shovel) return;
        PlayZapEffectServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void PlayZapEffectServerRpc()
    {
        PlayZapEffectClientRpc();
    }
    
    [ClientRpc]
    private void PlayZapEffectClientRpc()
    {
        zapEffect.Play();
    }
    
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        ActiveUkuleles.Remove(this);
    }
}