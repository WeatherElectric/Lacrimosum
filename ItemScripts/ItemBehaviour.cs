namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Item Behaviour")]
public class ItemBehaviour : GrabbableObject
{
    [Space(10f)]
    [Header("Extension Properties")]
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