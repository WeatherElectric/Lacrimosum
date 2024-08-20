namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Throwable Item Behaviour")]
public class ThrowableItemBehaviour : ItemBehaviour
{
    [Space(10f)]
    [Header("Throw Curve Settings")]
    public AnimationCurve itemFallCurve;
    public AnimationCurve itemVerticalFallCurveNoBounce;

    protected bool WasThrown;
    
    private RaycastHit _itemHit;
    private Ray _itemThrowRay;
    private const int StunGrenadeMask = 268437761;
    
    public override void ItemActivate(bool used, bool buttonDown = true)
    {
        base.ItemActivate(used, buttonDown);
        if (!IsOwner) return;
        playerHeldBy.DiscardHeldObject(placeObject: true, null, GetGrenadeThrowDestination());
        WasThrown = true;
    }
    
    public override void FallWithCurve()
    {
        var magnitude = (startFallingPosition - targetFloorPosition).magnitude;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(itemProperties.restingRotation.x, transform.eulerAngles.y, itemProperties.restingRotation.z), 14f * Time.deltaTime / magnitude);
        transform.localPosition = Vector3.Lerp(startFallingPosition, targetFloorPosition, itemFallCurve.Evaluate(fallTime));
        transform.localPosition = Vector3.Lerp(new Vector3(transform.localPosition.x, startFallingPosition.y, transform.localPosition.z), new Vector3(transform.localPosition.x, targetFloorPosition.y, transform.localPosition.z), magnitude > 5f ? itemVerticalFallCurveNoBounce.Evaluate(fallTime) : itemFallCurve.Evaluate(fallTime));
        fallTime += Mathf.Abs(Time.deltaTime * 12f / magnitude);
    }

    private Vector3 GetGrenadeThrowDestination()
    {
        Debug.DrawRay(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward, Color.yellow, 15f);
        _itemThrowRay = new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward);
        var position = !Physics.Raycast(_itemThrowRay, out _itemHit, 12f, StunGrenadeMask, QueryTriggerInteraction.Ignore) ? _itemThrowRay.GetPoint(10f) : _itemThrowRay.GetPoint(_itemHit.distance - 0.05f);
        Debug.DrawRay(position, Vector3.down, Color.blue, 15f);
        _itemThrowRay = new Ray(position, Vector3.down);
        if (Physics.Raycast(_itemThrowRay, out _itemHit, 30f, StunGrenadeMask, QueryTriggerInteraction.Ignore))
        {
            return _itemHit.point + Vector3.up * 0.05f;
        }
        return _itemThrowRay.GetPoint(30f);
    }
    
    public override void DiscardItem()
    {
        if (playerHeldBy != null)
        {
            playerHeldBy.equippedUsableItemQE = false;
        }
        isBeingUsed = false;
        base.DiscardItem();
    }
        
    public override void PocketItem()
    {
        if (playerHeldBy != null)
        {
            playerHeldBy.equippedUsableItemQE = false;
        }
        isBeingUsed = false;
        base.PocketItem();
    }
        
    public override void EquipItem()
    {
        base.EquipItem();
        playerHeldBy.equippedUsableItemQE = true;
    }

    public override void GrabItem()
    {
        base.GrabItem();
        WasThrown = false;
    }
}