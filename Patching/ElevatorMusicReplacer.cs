namespace Lacrimosum.Patching;

internal static class ElevatorMusicReplacer
{
    private static bool _ranOnce;
    private static Terminal _terminal;
    
    public static void Init()
    {
        On.MineshaftElevatorController.OnEnable += OnElevatorEnable;
        RoR2Plugin.ModConsole.LogDebug("ElevatorMusicReplacer: MineshaftElevatorController.OnEnable hooked!");
        On.StartOfRound.ShipLeave += OnShipLeave;
        RoR2Plugin.ModConsole.LogDebug("ElevatorMusicReplacer: StartOfRound.ShipLeave hooked!");
        On.Terminal.Start += OnTerminalStart;
    }
    
    private static void OnTerminalStart(On.Terminal.orig_Start orig, Terminal self)
    {
        orig(self);
        _terminal = self;
    }
    
    private static void OnShipLeave(On.StartOfRound.orig_ShipLeave orig, StartOfRound self)
    {
        orig(self);
        
        _ranOnce = false;
    }
    
    private static void OnElevatorEnable(On.MineshaftElevatorController.orig_OnEnable orig, MineshaftElevatorController self)
    {
        orig(self);
        
        if (_ranOnce) return;
        
        if (RoR2Plugin.ModConfig.AlwaysReplaceElevatorMusic)
        {
            self.elevatorJingleMusic.clip = ModAssets.ElevatorMusic;
            _ranOnce = true;
        }
        else
        {
            System.Random rnd = new(_terminal.groupCredits);
            var replace = rnd.Next(0, 100) < 2;
            if (!replace) return;
            self.elevatorJingleMusic.clip = ModAssets.ElevatorMusic;
            _ranOnce = true;
        }
    }
}