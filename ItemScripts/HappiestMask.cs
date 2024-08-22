namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Happiest Mask")]
public class HappiestMask : HauntedMaskItem
{
    public override void OnNetworkSpawn()
    {
        // Avoids having to pack duplicate assets into the mod's bundles. Why have a duplicate of the Masked enemy when I can just grab the one from the game in runtime?
        mimicEnemy = NetworkPrefabsHelper.MaskedEnemy;
    }
}