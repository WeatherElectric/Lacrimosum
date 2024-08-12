

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
        
        var goatHoof = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/PaulsGoatHoof.asset");
        NetworkPrefabs.RegisterNetworkPrefab(goatHoof.spawnPrefab);
        Items.RegisterScrap(goatHoof, RoR2Plugin.Config.GoatHoofSpawnWeight, Levels.LevelTypes.All);
        
        var whiteScrap = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/WhiteScrap.asset");
        NetworkPrefabs.RegisterNetworkPrefab(whiteScrap.spawnPrefab);
        Items.RegisterScrap(whiteScrap, RoR2Plugin.Config.WhiteScrapSpawnWeight, Levels.LevelTypes.All);
        
        var greenScrap = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/GreenScrap.asset");
        NetworkPrefabs.RegisterNetworkPrefab(greenScrap.spawnPrefab);
        Items.RegisterScrap(greenScrap, RoR2Plugin.Config.GreenScrapSpawnWeight, Levels.LevelTypes.All);
        
        var redScrap = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/RedScrap.asset");
        NetworkPrefabs.RegisterNetworkPrefab(redScrap.spawnPrefab);
        Items.RegisterScrap(redScrap, RoR2Plugin.Config.RedScrapSpawnWeight, Levels.LevelTypes.All);
        
        var yellowScrap = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/YellowScrap.asset");
        NetworkPrefabs.RegisterNetworkPrefab(yellowScrap.spawnPrefab);
        Items.RegisterScrap(yellowScrap, RoR2Plugin.Config.YellowScrapSpawnWeight, Levels.LevelTypes.All);
    }

    private static void LoadShopItems()
    {
        var diosBestFriend = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Shop/DiosBestFriend.asset");
        var diosBestFriendNode = _bundle.LoadAsset<TerminalNode>("Assets/Lacrimosum/Shop/DiosBestFriendNode.asset");
        NetworkPrefabs.RegisterNetworkPrefab(diosBestFriend.spawnPrefab);
        Items.RegisterShopItem(diosBestFriend, null!, null!, diosBestFriendNode, RoR2Plugin.Config.DiosBestFriendPrice);
    }
}