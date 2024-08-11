using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CSync.Extensions;
using CSync.Lib;
using Lacrimosum.Assets;

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
        WilloWispSpawnWeight =
            configFile.BindSyncedEntry(ScrapSection, "WilloWispSpawnWeight", 10,
                "Explode enemies on death, except you're the enemy");
        
        DiosBestFriendPrice =
            configFile.BindSyncedEntry(ShopSection, "DiosBestFriendPrice", 1000,
                "Cheat death. Consumed on use.");

        ConfigManager.Register(this);
    }
    
    [field: SyncedEntryField] public SyncedEntry<int> BisonSteakSpawnWeight { get; set; }
    [field: SyncedEntryField] public SyncedEntry<int> WilloWispSpawnWeight { get; set; }
    
    [field: SyncedEntryField] public SyncedEntry<int> DiosBestFriendPrice { get; set; }
}