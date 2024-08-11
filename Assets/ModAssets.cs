

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Lacrimosum.Assets;

internal static class ModAssets
{
    private static AssetBundle _bundle;

    public static void Load()
    {
        if (!LoadBundle("lacrimosum")) return;
        
        LoadScrapItems();
        LoadShopItems();
    }

    private static bool LoadBundle(string bundleName)
    {
        _bundle = AssetBundle.LoadFromFile(Path.Combine(RoR2Plugin.AssemblyLocation, bundleName));

        if (_bundle != null) return true;
        RoR2Plugin.ModConsole.LogError("Failed to load asset bundle: " + bundleName);
        return false;
    }

    private static void LoadScrapItems()
    {
        var bisonSteak = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/BisonSteak.asset");
        NetworkPrefabs.RegisterNetworkPrefab(bisonSteak.spawnPrefab);
        Items.RegisterScrap(bisonSteak, RoR2Plugin.Config.BisonSteakSpawnWeight, Levels.LevelTypes.All);
        
        var willoWisp = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/WilloWisp.asset");
        NetworkPrefabs.RegisterNetworkPrefab(willoWisp.spawnPrefab);
        Items.RegisterScrap(willoWisp, RoR2Plugin.Config.WilloWispSpawnWeight, Levels.LevelTypes.All);
    }

    private static void LoadShopItems()
    {
        var diosBestFriend = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Shop/DiosBestFriend.asset");
        var diosBestFriendNode = _bundle.LoadAsset<TerminalNode>("Assets/Lacrimosum/Shop/DiosBestFriendNode.asset");
        NetworkPrefabs.RegisterNetworkPrefab(diosBestFriend.spawnPrefab);
        Items.RegisterShopItem(diosBestFriend, null!, null!, diosBestFriendNode, RoR2Plugin.Config.DiosBestFriendPrice);
    }
}