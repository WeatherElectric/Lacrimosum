namespace Lacrimosum.Patching;

internal static class NetworkPrefabsHelper
{
    public static readonly List<GameObject> NetworkPrefabs = [];

    public static EnemyType MaskedEnemy;
    
    public static void Init()
    {
        On.GameNetworkManager.Start += OnNetworkManagerStart;
    }

    private static void OnNetworkManagerStart(On.GameNetworkManager.orig_Start orig, GameNetworkManager self)
    {
        orig(self);
        
        foreach (var prefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
        {
            NetworkPrefabs.Add(prefab.Prefab);
        }
        
        FindRequiredPrefabs();
    }

    private static void FindRequiredPrefabs()
    {
        var maskedEnemyPrefab = NetworkPrefabs.Find(prefab => prefab.name == "MaskedPlayerEnemy");
        MaskedEnemy = maskedEnemyPrefab.GetComponent<EnemyAI>().enemyType;
    }
}