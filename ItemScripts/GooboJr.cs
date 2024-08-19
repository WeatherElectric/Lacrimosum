namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Goobo Jr.")]
public class GooboJr : ThrowableItemBehaviour
{
    [Space(10f)]
    [Header("Goobo Jr. Settings")]
    [Tooltip("The material to apply to the masked enemy.")]
    public Material gooboJrMaterial;
    
    private GameObject _maskedEnemyPrefab;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _maskedEnemyPrefab = NetworkPrefabList.NetworkPrefabs.Find(prefab => prefab.name == "MaskedPlayerEnemy");
        RoR2Plugin.ModConsole.LogDebug($"Found MaskedPlayerEnemy prefab: {_maskedEnemyPrefab}");
    }

    public override void PlayDropSFX()
    {
        base.PlayDropSFX();
        if (!wasThrown) return;
        if (StartOfRound.Instance.currentLevel.PlanetName == "71 Gordion") return;
        SpawnMaskedEnemyServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SpawnMaskedEnemyServerRpc()
    {
        SpawnMaskedEnemyClientRpc();
    }
    
    [ClientRpc]
    private void SpawnMaskedEnemyClientRpc()
    {
        var masked = Instantiate(_maskedEnemyPrefab);
        masked.transform.position = transform.position;
        _maskedEnemyPrefab.GetComponent<NetworkObject>().Spawn();
        var renderers = masked.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.material = gooboJrMaterial;
        }
        NetworkObject.Despawn();
    }
}