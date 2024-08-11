namespace Lacrimosum.ItemScripts;

public class ItemBehaviour : GrabbableObject
{
    public PlayerControllerB lastPlayerHeldBy;
    protected new NetworkObject NetworkObject => GetComponent<NetworkObject>();
    
    public override void GrabItem()
    {
        base.GrabItem();
        lastPlayerHeldBy = playerHeldBy;
    }
}