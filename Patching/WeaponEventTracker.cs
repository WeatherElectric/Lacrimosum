namespace Lacrimosum.Patching;

internal static class WeaponEventTracker
{
    public static void Init()
    {
        On.Shovel.HitShovel += OnShovelHit;
        RoR2Plugin.ModConsole.LogDebug("WeaponEventTracker: Shovel.HitShovel hooked!");
    }
    
    private static void OnShovelHit(On.Shovel.orig_HitShovel orig, Shovel self, bool cancel)
    {
        RoR2Plugin.ModConsole.LogDebug($"{self.playerHeldBy.playerUsername} swung Shovel {self.gameObject.name}");
        orig(self, cancel);
        
        foreach (var weapon in Ukulele.ActiveUkuleles)
        {
            weapon.HandleShovelHit(self);
            RoR2Plugin.ModConsole.LogDebug($"HandleShovelHit for {weapon.gameObject.name} belonging to {self.playerHeldBy.playerUsername}");
        }
    }
}