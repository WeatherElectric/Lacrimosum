namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Item Behaviour")]
public class ItemBehaviour : GrabbableObject
{
    protected PlayerControllerB LastPlayerHeldBy;
    protected new NetworkObject NetworkObject => GetComponent<NetworkObject>();
    protected ScanNodeProperties ScanNodeProperties => GetComponentInChildren<ScanNodeProperties>();
    protected AudioSource AudioSource => GetComponent<AudioSource>();
    
    public override void GrabItem()
    {
        base.GrabItem();
        LastPlayerHeldBy = playerHeldBy;
    }
}