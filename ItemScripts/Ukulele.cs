using System;
using System.Linq;

namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Ukulele")]
public class Ukulele : ItemBehaviour
{
    public int shovelHitForce = 1;
    public bool reelingUp;
    public bool isHoldingButton;
    private RaycastHit rayHit;
    private Coroutine reelingUpCoroutine;
    private RaycastHit[] objectsHitByShovel;
    private List<RaycastHit> objectsHitByShovelList = new();
    public AudioClip reelUp;
    public AudioClip swing;
    public AudioClip[] hitSFX;
    public AudioSource shovelAudio;
    private PlayerControllerB previousPlayerHeldBy;
    private readonly int shovelMask = 1084754248;

    [Header("Ukulele Settings")] 
    [Tooltip("The particles to play when the ukulele hits something")]
    public ParticleSystem zapEffect;

    public override void ItemActivate(bool used, bool buttonDown = true)
    {
        if (playerHeldBy == null)
            return;
        isHoldingButton = buttonDown;
        if (reelingUp || !buttonDown)
            return;
        reelingUp = true;
        previousPlayerHeldBy = playerHeldBy;
        if (reelingUpCoroutine != null)
            StopCoroutine(reelingUpCoroutine);
        reelingUpCoroutine = StartCoroutine(reelUpShovel());
    }

    private IEnumerator reelUpShovel()
    {
        var shovel = this;
        shovel.playerHeldBy.activatingItem = true;
        shovel.playerHeldBy.twoHanded = true;
        shovel.playerHeldBy.playerBodyAnimator.ResetTrigger("shovelHit");
        shovel.playerHeldBy.playerBodyAnimator.SetBool("reelingUp", true);
        shovel.shovelAudio.PlayOneShot(shovel.reelUp);
        shovel.ReelUpSFXServerRpc();
        yield return new WaitForSeconds(0.35f);
        // ISSUE: reference to a compiler-generated method
        shovel.SwingShovel(!shovel.isHeld);
        yield return new WaitForSeconds(0.13f);
        yield return new WaitForEndOfFrame();
        shovel.HitShovel(!shovel.isHeld);
        yield return new WaitForSeconds(0.3f);
        shovel.reelingUp = false;
        shovel.reelingUpCoroutine = null;
    }

    [ServerRpc]
    public void ReelUpSFXServerRpc()
    {
        ReelUpSFXClientRpc();
    }

    [ClientRpc]
    public void ReelUpSFXClientRpc()
    {
        shovelAudio.PlayOneShot(reelUp);
    }

    public override void DiscardItem()
    {
        if (playerHeldBy != null)
            playerHeldBy.activatingItem = false;
        base.DiscardItem();
    }

    public void SwingShovel(bool cancel = false)
    {
        previousPlayerHeldBy.playerBodyAnimator.SetBool("reelingUp", false);
        if (cancel)
            return;
        shovelAudio.PlayOneShot(swing);
        previousPlayerHeldBy.UpdateSpecialAnimationValue(true, (short)previousPlayerHeldBy.transform.localEulerAngles.y,
            0.4f);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void HitShovel(bool cancel = false)
    {
        if (previousPlayerHeldBy == null)
        {
            Debug.LogError("Previousplayerheldby is null on this client when HitShovel is called.");
        }
        else
        {
            previousPlayerHeldBy.activatingItem = false;
            var flag1 = false;
            var flag2 = false;
            var flag3 = false;
            var hitSurfaceID = -1;
            if (!cancel)
            {
                previousPlayerHeldBy.twoHanded = false;
                this.objectsHitByShovel = Physics.SphereCastAll(
                    previousPlayerHeldBy.gameplayCamera.transform.position +
                    previousPlayerHeldBy.gameplayCamera.transform.right * -0.35f, 0.8f,
                    previousPlayerHeldBy.gameplayCamera.transform.forward, 1.5f, shovelMask,
                    QueryTriggerInteraction.Collide);
                objectsHitByShovelList = this.objectsHitByShovel.OrderBy(x => x.distance).ToList();
                var enemyAiList = new List<EnemyAI>();
                for (var index1 = 0; index1 < objectsHitByShovelList.Count; ++index1)
                {
                    var objectsHitByShovel = objectsHitByShovelList[index1];
                    if (objectsHitByShovel.transform.gameObject.layer != 8)
                    {
                        objectsHitByShovel = objectsHitByShovelList[index1];
                        if (objectsHitByShovel.transform.gameObject.layer != 11)
                        {
                            objectsHitByShovel = objectsHitByShovelList[index1];
                            IHittable component1;
                            if (objectsHitByShovel.transform.TryGetComponent(out component1))
                            {
                                objectsHitByShovel = objectsHitByShovelList[index1];
                                if (!(objectsHitByShovel.transform == previousPlayerHeldBy.transform))
                                {
                                    objectsHitByShovel = objectsHitByShovelList[index1];
                                    if (!(objectsHitByShovel.point == Vector3.zero))
                                    {
                                        var position = previousPlayerHeldBy.gameplayCamera.transform.position;
                                        objectsHitByShovel = objectsHitByShovelList[index1];
                                        var point = objectsHitByShovel.point;
                                        RaycastHit raycastHit = default;
                                        ref var local = ref raycastHit;
                                        var roomMaskAndDefault = StartOfRound.Instance.collidersAndRoomMaskAndDefault;
                                        if (Physics.Linecast(position, point, out local, roomMaskAndDefault,
                                                QueryTriggerInteraction.Ignore))
                                            continue;
                                    }

                                    flag1 = true;
                                    var forward = previousPlayerHeldBy.gameplayCamera.transform.forward;
                                    try
                                    {
                                        objectsHitByShovel = objectsHitByShovelList[index1];
                                        var component2 = objectsHitByShovel.transform
                                            .GetComponent<EnemyAICollisionDetect>();
                                        if (component2)
                                        {
                                            if (!(!component2.mainScript))
                                            {
                                                if (enemyAiList.Contains(component2.mainScript))
                                                    continue;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            objectsHitByShovel = objectsHitByShovelList[index1];
                                            if (objectsHitByShovel.transform.GetComponent<PlayerControllerB>())
                                            {
                                                if (!flag3)
                                                    flag3 = true;
                                                else
                                                    continue;
                                            }
                                        }

                                        var flag4 = component1.Hit(shovelHitForce, forward, previousPlayerHeldBy, true,
                                            1);
                                        if (flag4 && component2 != null)
                                            enemyAiList.Add(component2.mainScript);
                                        if (!flag2)
                                        {
                                            flag2 = flag4;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.Log(string.Format(
                                            "Exception caught when hitting object with shovel from player #{0}: {1}",
                                            previousPlayerHeldBy.playerClientId, ex));
                                    }
                                }
                            }

                            continue;
                        }
                    }

                    objectsHitByShovel = objectsHitByShovelList[index1];
                    if (!objectsHitByShovel.collider.isTrigger)
                    {
                        flag1 = true;
                        objectsHitByShovel = objectsHitByShovelList[index1];
                        var tag = objectsHitByShovel.collider.gameObject.tag;
                        for (var index2 = 0; index2 < StartOfRound.Instance.footstepSurfaces.Length; ++index2)
                            if (StartOfRound.Instance.footstepSurfaces[index2].surfaceTag == tag)
                            {
                                hitSurfaceID = index2;
                                break;
                            }
                    }
                }
            }

            if (!flag1)
                return;
            RoundManager.PlayRandomClip(shovelAudio, hitSFX);
            FindObjectOfType<RoundManager>().PlayAudibleNoise(transform.position, 17f, 0.8f);
            if (!flag2 && hitSurfaceID != -1)
            {
                shovelAudio.PlayOneShot(StartOfRound.Instance.footstepSurfaces[hitSurfaceID].hitSurfaceSFX);
                WalkieTalkie.TransmitOneShotAudio(shovelAudio,
                    StartOfRound.Instance.footstepSurfaces[hitSurfaceID].hitSurfaceSFX);
            }

            playerHeldBy.playerBodyAnimator.SetTrigger("shovelHit");
            HitShovelServerRpc(hitSurfaceID);
        }
    }

    [ServerRpc]
    public void HitShovelServerRpc(int hitSurfaceID)
    {
        HitShovelClientRpc(hitSurfaceID);
    }

    [ClientRpc]
    public void HitShovelClientRpc(int hitSurfaceID)
    {
        RoundManager.PlayRandomClip(shovelAudio, hitSFX);
        if (hitSurfaceID == -1)
            return;
        HitSurfaceWithShovel(hitSurfaceID);
    }

    private void HitSurfaceWithShovel(int hitSurfaceID)
    {
        PlayZapEffectServerRpc();
        shovelAudio.PlayOneShot(StartOfRound.Instance.footstepSurfaces[hitSurfaceID].hitSurfaceSFX);
        WalkieTalkie.TransmitOneShotAudio(shovelAudio,
            StartOfRound.Instance.footstepSurfaces[hitSurfaceID].hitSurfaceSFX);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayZapEffectServerRpc()
    {
        PlayZapEffectClientRpc();
    }

    [ClientRpc]
    private void PlayZapEffectClientRpc()
    {
        zapEffect.Play();
    }
}