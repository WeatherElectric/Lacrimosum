using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CSync.Extensions;
using CSync.Lib;
using Lacrimosum.Assets;
using Lacrimosum.Patching;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Lacrimosum;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency("evaisa.lethallib")]
[BepInDependency("com.sigurd.csync", "5.0.1")]
internal class RoR2Plugin : BaseUnityPlugin
{
    private const string PluginName = "Lacrimosum";
#if DEBUG
    private const string PluginVersion = "1.0.4-DEBUG";
#else
    private const string PluginVersion = "1.0.4";
#endif
    internal const string PluginGuid = "fragiledeviations.lacrimosum";
    
    internal static readonly string AssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    internal static ManualLogSource ModConsole;
    internal static LacrimosumConfig ModConfig;

    private void Awake()
    {
        ModConsole = Logger;
        
        ModConsole.LogInfo($"Loading {PluginGuid}...");
#if DEBUG
        ModConsole.LogWarning("This is a debug build! Expect problems!");
#endif

        ModConfig = new LacrimosumConfig(Config);
        ModConsole.LogInfo($"Config loaded for {PluginGuid}");

        NetcodePatcher();
        ModConsole.LogInfo($"Netcode patched for {PluginGuid}");

        ModAssets.Load();
        ModConsole.LogInfo($"Assets loaded for {PluginGuid}");
        PlayerEventTracker.Init();
        ModConsole.LogInfo("PlayerEventTracker loaded");
        WeaponEventTracker.Init();
        ModConsole.LogInfo("WeaponEventTracker loaded");
        NetworkPrefabsHelper.Init();
        ModConsole.LogInfo("NetworkPrefabsHelper loaded");
        MenuMusicReplacer.Init();
        ModConsole.LogInfo("MenuMusicReplacer loaded");
        MineReplacement.Init();
        ModConsole.LogInfo("MineReplacement loaded");
    }

    private static void NetcodePatcher()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                if (attributes.Length > 0) method.Invoke(null, null);
            }
        }
    }
}

internal class LacrimosumConfig : SyncedConfig2<LacrimosumConfig>
{
    private const string GeneralSection = "General";
    private const string ScrapSection = "Scrap";
    private const string ShopSection = "Shop Items";
    
    public LacrimosumConfig(ConfigFile configFile) : base(RoR2Plugin.PluginGuid)
    {
        #region General

        AlwaysReplaceMenuMusic =
            configFile.Bind(GeneralSection, "AlwaysReplaceMenuMusic", false,
                "Always replace the menu music with the ROR2 menu music.");
        DontOverrideMineCode =
            configFile.BindSyncedEntry(GeneralSection, "DontOverrideMineCode", false,
                "Don't override the mine explosion code. Having this enabled will make Safer Spaces unable to save you from mines.");

        #endregion
        
        #region Scrap
        
        WilloWispSpawnWeight =
            configFile.BindSyncedEntry(ScrapSection, "WilloWispSpawnWeight", 20,
                "Explode enemies on death, except you're the enemy");
        GoatHoofSpawnWeight =
            configFile.BindSyncedEntry(ScrapSection, "GoatHoofSpawnWeight", 20,
                "Increase movement speed.");
        GoatHoofSpeedIncrease =
            configFile.BindSyncedEntry(ScrapSection, "GoatHoofSpeedIncrease", 4.6f,
                "How much to increase movement speed by. The player's base speed is 4.6, for reference.");
        WhiteScrapSpawnWeight =
            configFile.BindSyncedEntry(ScrapSection, "WhiteScrapSpawnWeight", 40,
                "Does nothing.");
        GreenScrapSpawnWeight =
            configFile.BindSyncedEntry(ScrapSection, "GreenScrapSpawnWeight", 30,
                "Does nothing.");
        YellowScrapSpawnWeight =
            configFile.BindSyncedEntry(ScrapSection, "YellowScrapSpawnWeight", 20,
                "Does nothing.");
        RedScrapSpawnWeight =
            configFile.BindSyncedEntry(ScrapSection, "RedScrapSpawnWeight", 10,
                "Does nothing.");
        BungusSpawnWeight =
            configFile.BindSyncedEntry(ScrapSection, "BungusSpawnWeight", 20,
                "Heal when standing still.");
        BungusHealthIncrease =
            configFile.BindSyncedEntry(ScrapSection, "BungusHealthIncrease", 5,
                "How much health to increase per heal tick.");
        BungusHealInterval =
            configFile.BindSyncedEntry(ScrapSection, "BungusHealInterval", 1f,
                "How often to heal.");
        BungusActivationTime =
            configFile.BindSyncedEntry(ScrapSection, "BungusActivationTime", 1f,
                "How long to stand still before healing.");
        BungusMode =
            configFile.BindSyncedEntry(ScrapSection, "BungusMode", false,
                "Rename the item to Bungus. If false, it's called Bustling Fungus.");
        RollOfPenniesSpawnWeight =
            configFile.BindSyncedEntry(ScrapSection, "RollOfPenniesSpawnWeight", 40,
                "Increases item value upon taking damage.");
        RollOfPenniesValueIncrease =
            configFile.BindSyncedEntry(ScrapSection, "RollOfPenniesValueIncrease", 20,
                "How much to increase item value by.");
        RollOfPenniesMaxValue =
            configFile.BindSyncedEntry(ScrapSection, "RollOfPenniesMaxValue", 100,
                "The maximum value the item can reach.");
        UkuleleSpawnWeight =
            configFile.BindSyncedEntry(ScrapSection, "UkuleleSpawnWeight", 30,
                "...and his music was electric.");
        GooboJrSpawnWeight =
            configFile.BindSyncedEntry(ScrapSection, "GooboJrSpawnWeight", 10,
                "Spawn a gummy clone of yourself. May not be friendly.");
        
        #endregion
        
        #region Shop
        
        SaferSpacesPrice =
            configFile.BindSyncedEntry(ShopSection, "SaferSpacesPrice", 800,
                "Block one incoming hit. Recharges after a short delay.");
        SaferSpacesCooldown =
            configFile.BindSyncedEntry(ShopSection, "SaferSpacesCooldown", 3f,
                "How long to wait before recharging the block.");
        
        #endregion
        
        ConfigManager.Register(this);
    }
    
    public ConfigEntry<bool> AlwaysReplaceMenuMusic { get; set; }
    public SyncedEntry<bool> DontOverrideMineCode { get; set; }
    
    [field: SyncedEntryField] public SyncedEntry<int> WilloWispSpawnWeight { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> GoatHoofSpawnWeight { get; set; }
    [field: SyncedEntryField] public SyncedEntry<float> GoatHoofSpeedIncrease { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> WhiteScrapSpawnWeight { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> GreenScrapSpawnWeight { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> RedScrapSpawnWeight { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> YellowScrapSpawnWeight { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> BungusSpawnWeight { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> BungusHealthIncrease { get; set; }
    [field: SyncedEntryField] public SyncedEntry<float> BungusHealInterval { get; set; }
    [field: SyncedEntryField] public SyncedEntry<float> BungusActivationTime { get; set; }
    [field: SyncedEntryField] public SyncedEntry<bool> BungusMode { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> RollOfPenniesSpawnWeight { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> RollOfPenniesValueIncrease { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> RollOfPenniesMaxValue { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> UkuleleSpawnWeight { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> GooboJrSpawnWeight { get; set; }
    
    [field: SyncedEntryField] public SyncedEntry<int> SaferSpacesPrice { get; set; }
    [field: SyncedEntryField] public SyncedEntry<float> SaferSpacesCooldown { get; set; }
}