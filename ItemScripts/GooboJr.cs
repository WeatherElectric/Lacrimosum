namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Goobo Jr.")]
public class GooboJr : ThrowableItemBehaviour
{
    [Space(10f)]
    [Header("Goobo Jr. Settings")]
    [Tooltip("The material to apply to the masked enemy.")]
    public Material gooboJrMaterial;
    
    private GameObject _maskedEnemyPrefab;
    private EnemyType _enemyType;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _maskedEnemyPrefab = NetworkPrefabList.NetworkPrefabs.Find(prefab => prefab.name == "MaskedPlayerEnemy");
        _enemyType = _maskedEnemyPrefab.GetComponent<EnemyAI>().enemyType;
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
        RoundManager.Instance.SpawnEnemyGameObject(transform.position, transform.rotation.y, 0, _enemyType);
        NetworkObject.Despawn();
    }
}