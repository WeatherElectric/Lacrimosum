namespace Lacrimosum.Helpers;

internal static class NetworkPrefabList
{
    public static readonly List<GameObject> NetworkPrefabs = [];
    
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
    }
}