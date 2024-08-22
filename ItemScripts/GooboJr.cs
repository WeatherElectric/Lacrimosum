namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Goobo Jr.")]
public class GooboJr : ThrowableItemBehaviour
{
    [Space(10f)]
    [Header("Goobo Jr. Settings")]
    [Tooltip("The material to apply to the masked enemy.")]
    public Material gooboJrMaterial;
    [Tooltip("The sounds to play when a Goobo is spawned.")]
    public AudioClip[] gooboSpawnSounds;

    public override void PlayDropSFX()
    {
        base.PlayDropSFX();
        if (!WasThrown) return;
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
        RoundManager.Instance.SpawnEnemyGameObject(transform.position, transform.rotation.y, 0, NetworkPrefabsHelper.MaskedEnemy);
        var maskedEnemies = FindObjectsOfType<MaskedPlayerEnemy>();
        foreach (var enemy in maskedEnemies)
        {
            var distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance > 2f) continue;
            var skinnedMeshRenderers = enemy.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                skinnedMeshRenderer.material = gooboJrMaterial;
            }
        }
        AudioSource.PlayClipAtPoint(gooboSpawnSounds[Random.Range(0, gooboSpawnSounds.Length)], transform.position);
        NetworkObject.Despawn();
    }
}