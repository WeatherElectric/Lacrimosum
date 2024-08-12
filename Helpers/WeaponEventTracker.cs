using Lacrimosum.ItemScripts;

namespace Lacrimosum.Helpers;

internal static class WeaponEventTracker
{
    public static void Init()
    {
        On.Shovel.HitShovel += OnShovelHit;
    }
    
    private static void OnShovelHit(On.Shovel.orig_HitShovel orig, Shovel self, bool cancel)
    {
        orig(self, cancel);
        
        foreach (var weapon in Ukulele.ActiveUkuleles)
        {
            weapon.HandleShovelHit(self);
        }
    }
}