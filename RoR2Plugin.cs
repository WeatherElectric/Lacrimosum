using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CSync.Extensions;
using CSync.Lib;
using Lacrimosum.Assets;
using Lacrimosum.ItemScripts;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Lacrimosum;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency("evaisa.lethallib")]
[BepInDependency("com.sigurd.csync", "5.0.1")]
internal class RoR2Plugin : BaseUnityPlugin
{
    private const string PluginName = "Lacrimosum";
#if DEBUG
    private const string PluginVersion = "1.0.0-DEBUG";
#else
    private const string PluginVersion = "1.0.0";
#endif
    internal const string PluginGuid = "fragiledeviations.lacrimosum";
    
    internal static readonly string AssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    internal static ManualLogSource ModConsole;
    internal new static LacrimosumConfig Config;

    private void Awake()
    {
        ModConsole = Logger;

        ModConsole.LogInfo($"Plugin {PluginGuid} is loaded!");
#if DEBUG
        ModConsole.LogWarning("This is a debug build! Expect shit to be dumber than normal!");
#endif

        Config = new LacrimosumConfig(base.Config);

        NetcodePatcher();

        ModAssets.Load();
        PlayerDeathTracker.Init();
        BungusHelper.Init();
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
    private const string ScrapSection = "Scrap";
    private const string ShopSection = "Shop Items";
    
    public LacrimosumConfig(ConfigFile configFile) : base(RoR2Plugin.PluginGuid)
    {
        BisonSteakSpawnWeight =
            configFile.BindSyncedEntry(ScrapSection, "BisonSteakSpawnWeight", 10,
                "The most useless item other than the shitass old war stealth kit!");
        BisonSteakHealthIncrease =
            configFile.BindSyncedEntry(ScrapSection, "BisonSteakHealthIncrease", 25,
                "How much health to increase by. Player's base health is 100, for reference.");
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
        
        
        
        DiosBestFriendPrice =
            configFile.BindSyncedEntry(ShopSection, "DiosBestFriendPrice", 1000,
                "Cheat death. Consumed on use.");
        DiosBestFriendRespawnAtShip =
            configFile.BindSyncedEntry(ShopSection, "DiosBestFriendRespawnAtShip", true,
                "Respawn at the ship when used. Otherwise, respawn where you died. If disabled and you die to a pit or mud or water, you will stay there and just die again. Recommended to leave true unless you're too good and never die to those.");

        ConfigManager.Register(this);
    }
    
    [field: SyncedEntryField] public SyncedEntry<int> BisonSteakSpawnWeight { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> BisonSteakHealthIncrease { get; set; }
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
    
    [field: SyncedEntryField] public SyncedEntry<int> DiosBestFriendPrice { get; set; }
    [field: SyncedEntryField] public SyncedEntry<bool> DiosBestFriendRespawnAtShip { get; set; }
}