

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Lacrimosum.Assets;

internal static class ModAssets
{
    private static AssetBundle _bundle;
    
    public static GameObject BungusMushroomWardPrefab;
    public static GameObject SaferSpacesBubblePrefab;
    
    public static AudioClip ElevatorMusic;
    public static AudioClip RoR2MenuMusic;
    
    public static GameObject LilGuyPrefab;
    public static Sprite BlueLogo;

    public static void Load()
    {
        if (!LoadBundle("lacrimosum")) return;
        
        if (RoR2Plugin.ModConfig.EnableShopItems) LoadShopItems();
        LoadScrapItems();
        LoadStrayAssets();
    }

    private static bool LoadBundle(string bundleName)
    {
        _bundle = AssetBundle.LoadFromFile(Path.Combine(RoR2Plugin.AssemblyLocation, bundleName));

        if (_bundle != null) return true;
        RoR2Plugin.ModConsole.LogError("Failed to load asset bundle: " + bundleName);
        return false;
    }

    private static void LoadStrayAssets()
    {
        BungusMushroomWardPrefab = _bundle.LoadAsset<GameObject>("Assets/Lacrimosum/Scrap/BungusAssets/MushroomWard.prefab");
        BungusMushroomWardPrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;
        
        SaferSpacesBubblePrefab = _bundle.LoadAsset<GameObject>("Assets/Lacrimosum/Shop/SaferSpacesAssets/Bubble.prefab");
        SaferSpacesBubblePrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;
        
        RoR2MenuMusic = _bundle.LoadAsset<AudioClip>("Assets/Lacrimosum/Menu/RiskOfRain2.flac");
        RoR2MenuMusic.hideFlags = HideFlags.DontUnloadUnusedAsset;
        
        ElevatorMusic = _bundle.LoadAsset<AudioClip>("Assets/Lacrimosum/Elevator/Coalescence.wav");
        ElevatorMusic.hideFlags = HideFlags.DontUnloadUnusedAsset;
        
        BlueLogo = _bundle.LoadAsset<Sprite>("Assets/Lacrimosum/Menu/BlueLogo.png");
        BlueLogo.hideFlags = HideFlags.DontUnloadUnusedAsset;
        
        LilGuyPrefab = _bundle.LoadAsset<GameObject>("Assets/Lacrimosum/Menu/LilGuy.prefab");
        LilGuyPrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    private const Levels.LevelTypes GooboSpawnMaps = Levels.LevelTypes.RendLevel | Levels.LevelTypes.TitanLevel |
                                                     Levels.LevelTypes.ArtificeLevel | Levels.LevelTypes.DineLevel |
                                                     Levels.LevelTypes.AssuranceLevel | Levels.LevelTypes.Modded;

    private const Levels.LevelTypes BungusSpawnMaps = Levels.LevelTypes.Modded | Levels.LevelTypes.AdamanceLevel |
                                                      Levels.LevelTypes.MarchLevel | Levels.LevelTypes.VowLevel |
                                                      Levels.LevelTypes.DineLevel | Levels.LevelTypes.RendLevel;

    private const Levels.LevelTypes HappiestMaskSpawnMaps = Levels.LevelTypes.RendLevel | Levels.LevelTypes.TitanLevel |
                                                            Levels.LevelTypes.ArtificeLevel |
                                                            Levels.LevelTypes.DineLevel |
                                                            Levels.LevelTypes.AssuranceLevel |
                                                            Levels.LevelTypes.Modded | Levels.LevelTypes.EmbrionLevel;
    
    private static void LoadScrapItems()
    {
        var willoWisp = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/WilloWisp.asset");
        NetworkPrefabs.RegisterNetworkPrefab(willoWisp.spawnPrefab);
        Items.RegisterScrap(willoWisp, RoR2Plugin.ModConfig.WilloWispSpawnWeight, Levels.LevelTypes.All);
        
        var goatHoof = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/PaulsGoatHoof.asset");
        NetworkPrefabs.RegisterNetworkPrefab(goatHoof.spawnPrefab);
        Items.RegisterScrap(goatHoof, RoR2Plugin.ModConfig.GoatHoofSpawnWeight, Levels.LevelTypes.All);
        
        var whiteScrap = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/WhiteScrap.asset");
        NetworkPrefabs.RegisterNetworkPrefab(whiteScrap.spawnPrefab);
        Items.RegisterScrap(whiteScrap, RoR2Plugin.ModConfig.WhiteScrapSpawnWeight, Levels.LevelTypes.All);
        
        var greenScrap = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/GreenScrap.asset");
        NetworkPrefabs.RegisterNetworkPrefab(greenScrap.spawnPrefab);
        Items.RegisterScrap(greenScrap, RoR2Plugin.ModConfig.GreenScrapSpawnWeight, Levels.LevelTypes.All);
        
        var redScrap = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/RedScrap.asset");
        NetworkPrefabs.RegisterNetworkPrefab(redScrap.spawnPrefab);
        Items.RegisterScrap(redScrap, RoR2Plugin.ModConfig.RedScrapSpawnWeight, Levels.LevelTypes.All);
        
        var yellowScrap = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/YellowScrap.asset");
        NetworkPrefabs.RegisterNetworkPrefab(yellowScrap.spawnPrefab);
        Items.RegisterScrap(yellowScrap, RoR2Plugin.ModConfig.YellowScrapSpawnWeight, Levels.LevelTypes.All);
        
        var bungus = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/Bungus.asset");
        if (RoR2Plugin.ModConfig.BungusMode) bungus.itemName = "Bungus";
        NetworkPrefabs.RegisterNetworkPrefab(bungus.spawnPrefab);
        Items.RegisterScrap(bungus, RoR2Plugin.ModConfig.BungusSpawnWeight, BungusSpawnMaps);
        
        var rollOfPennies = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/RollOfPennies.asset");
        NetworkPrefabs.RegisterNetworkPrefab(rollOfPennies.spawnPrefab);
        Items.RegisterScrap(rollOfPennies, RoR2Plugin.ModConfig.RollOfPenniesSpawnWeight, Levels.LevelTypes.All);
        
        var ukulele = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/Ukulele.asset");
        NetworkPrefabs.RegisterNetworkPrefab(ukulele.spawnPrefab);
        Items.RegisterScrap(ukulele, RoR2Plugin.ModConfig.UkuleleSpawnWeight, Levels.LevelTypes.All);
        
        var goobojr = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/GooboJr.asset");
        NetworkPrefabs.RegisterNetworkPrefab(goobojr.spawnPrefab);
        Items.RegisterScrap(goobojr, RoR2Plugin.ModConfig.GooboJrSpawnWeight, GooboSpawnMaps);
        
        var happiestMask = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Scrap/HappiestMask.asset");
        NetworkPrefabs.RegisterNetworkPrefab(happiestMask.spawnPrefab);
        Items.RegisterScrap(happiestMask, RoR2Plugin.ModConfig.HappiestMaskSpawnWeight, HappiestMaskSpawnMaps);
    }

    private static void LoadShopItems()
    {
        var saferSpaces = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Shop/SaferSpaces.asset");
        var saferSpacesNode = _bundle.LoadAsset<TerminalNode>("Assets/Lacrimosum/Shop/SaferSpacesNode.asset");
        NetworkPrefabs.RegisterNetworkPrefab(saferSpaces.spawnPrefab);
        Items.RegisterShopItem(saferSpaces, null!, null!, saferSpacesNode, RoR2Plugin.ModConfig.SaferSpacesPrice);
        
        var powerElixir = _bundle.LoadAsset<Item>("Assets/Lacrimosum/Shop/PowerElixir.asset");
        var powerElixirNode = _bundle.LoadAsset<TerminalNode>("Assets/Lacrimosum/Shop/PowerElixirNode.asset");
        NetworkPrefabs.RegisterNetworkPrefab(powerElixir.spawnPrefab);
        Items.RegisterShopItem(powerElixir, null!, null!, powerElixirNode, RoR2Plugin.ModConfig.PowerElixirPrice);
    }
}