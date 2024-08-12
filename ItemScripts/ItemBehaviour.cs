namespace Lacrimosum.ItemScripts;

public class ItemBehaviour : GrabbableObject
{
    public PlayerControllerB lastPlayerHeldBy;
    protected new NetworkObject NetworkObject => GetComponent<NetworkObject>();
    protected ScanNodeProperties ScanNodeProperties => GetComponentInChildren<ScanNodeProperties>();
    protected AudioSource AudioSource => GetComponent<AudioSource>();
    
    public override void GrabItem()
    {
        base.GrabItem();
        lastPlayerHeldBy = playerHeldBy;
    }
}