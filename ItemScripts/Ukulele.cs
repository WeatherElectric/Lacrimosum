using System;
using System.Linq;
using UnityEngine.Serialization;
// ReSharper disable Unity.PreferAddressByIdToGraphicsParams

namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Ukulele")]
public class Ukulele : ItemBehaviour
{
    public int shovelHitForce = 1;
    public bool reelingUp;
    public bool isHoldingButton;
    private RaycastHit _rayHit;
    private Coroutine _reelingUpCoroutine;
    private RaycastHit[] _objectsHitByShovel;
    private List<RaycastHit> _objectsHitByShovelList = [];
    public AudioClip reelUp;
    public AudioClip swing;
    [FormerlySerializedAs("hitSFX")] public AudioClip[] hitSfx;
    public AudioSource shovelAudio;
    private PlayerControllerB _previousPlayerHeldBy;
    private const int ShovelMask = 1084754248;

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
        _previousPlayerHeldBy = playerHeldBy;
        if (_reelingUpCoroutine != null)
            StopCoroutine(_reelingUpCoroutine);
        _reelingUpCoroutine = StartCoroutine(ReelUpShovel());
    }

    private IEnumerator ReelUpShovel()
    {
        var shovel = this;
        shovel.playerHeldBy.activatingItem = true;
        shovel.playerHeldBy.twoHanded = true;
        shovel.playerHeldBy.playerBodyAnimator.ResetTrigger("shovelHit");
        shovel.playerHeldBy.playerBodyAnimator.SetBool("reelingUp", true);
        shovel.shovelAudio.PlayOneShot(shovel.reelUp);
        shovel.ReelUpSfxServerRpc();
        yield return new WaitForSeconds(0.35f);
        yield return new WaitUntil(() => !shovel.isHoldingButton);
        shovel.SwingShovel(!shovel.isHeld);
        yield return new WaitForSeconds(0.13f);
        yield return new WaitForEndOfFrame();
        shovel.HitShovel(!shovel.isHeld);
        yield return new WaitForSeconds(0.3f);
        shovel.reelingUp = false;
        shovel._reelingUpCoroutine = null;
    }

    [ServerRpc]
    public void ReelUpSfxServerRpc()
    {
        ReelUpSfxClientRpc();
    }

    [ClientRpc]
    public void ReelUpSfxClientRpc()
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
        _previousPlayerHeldBy.playerBodyAnimator.SetBool("reelingUp", false);
        if (cancel)
            return;
        shovelAudio.PlayOneShot(swing);
        _previousPlayerHeldBy.UpdateSpecialAnimationValue(true, (short)_previousPlayerHeldBy.transform.localEulerAngles.y,
            0.4f);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void HitShovel(bool cancel = false)
    {
        if (_previousPlayerHeldBy == null)
        {
            Debug.LogError("Previousplayerheldby is null on this client when HitShovel is called.");
        }
        else
        {
            _previousPlayerHeldBy.activatingItem = false;
            var flag1 = false;
            var flag2 = false;
            var flag3 = false;
            var hitSurfaceID = -1;
            if (!cancel)
            {
                _previousPlayerHeldBy.twoHanded = false;
                // ReSharper disable once Unity.PreferNonAllocApi
                this._objectsHitByShovel = Physics.SphereCastAll(
                    _previousPlayerHeldBy.gameplayCamera.transform.position +
                    _previousPlayerHeldBy.gameplayCamera.transform.right * -0.35f, 0.8f,
                    _previousPlayerHeldBy.gameplayCamera.transform.forward, 1.5f, ShovelMask,
                    QueryTriggerInteraction.Collide);
                _objectsHitByShovelList = this._objectsHitByShovel.OrderBy(x => x.distance).ToList();
                var enemyAiList = new List<EnemyAI>();
                foreach (var t in _objectsHitByShovelList)
                {
                    var objectsHitByShovel = t;
                    if (objectsHitByShovel.transform.gameObject.layer != 8)
                    {
                        objectsHitByShovel = t;
                        if (objectsHitByShovel.transform.gameObject.layer != 11)
                        {
                            objectsHitByShovel = t;
                            if (objectsHitByShovel.transform.TryGetComponent(out IHittable component1))
                            {
                                objectsHitByShovel = t;
                                if (!(objectsHitByShovel.transform == _previousPlayerHeldBy.transform))
                                {
                                    objectsHitByShovel = t;
                                    if (!(objectsHitByShovel.point == Vector3.zero))
                                    {
                                        var position = _previousPlayerHeldBy.gameplayCamera.transform.position;
                                        objectsHitByShovel = t;
                                        var point = objectsHitByShovel.point;
                                        RaycastHit raycastHit = default;
                                        ref var local = ref raycastHit;
                                        var roomMaskAndDefault = StartOfRound.Instance.collidersAndRoomMaskAndDefault;
                                        if (Physics.Linecast(position, point, out local, roomMaskAndDefault,
                                                QueryTriggerInteraction.Ignore))
                                            continue;
                                    }

                                    flag1 = true;
                                    var forward = _previousPlayerHeldBy.gameplayCamera.transform.forward;
                                    try
                                    {
                                        objectsHitByShovel = t;
                                        var component2 = objectsHitByShovel.transform
                                            .GetComponent<EnemyAICollisionDetect>();
                                        if (component2)
                                        {
                                            if (component2.mainScript)
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
                                            objectsHitByShovel = t;
                                            if (objectsHitByShovel.transform.GetComponent<PlayerControllerB>())
                                            {
                                                if (!flag3)
                                                    flag3 = true;
                                                else
                                                    continue;
                                            }
                                        }

                                        var flag4 = component1.Hit(shovelHitForce, forward, _previousPlayerHeldBy, true,
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
                                            _previousPlayerHeldBy.playerClientId, ex));
                                    }
                                }
                            }

                            continue;
                        }
                    }

                    objectsHitByShovel = t;
                    if (objectsHitByShovel.collider.isTrigger) continue;
                    flag1 = true;
                    objectsHitByShovel = t;
                    var gameObjectTag = objectsHitByShovel.collider.gameObject.tag;
                    for (var index2 = 0; index2 < StartOfRound.Instance.footstepSurfaces.Length; ++index2)
                        if (StartOfRound.Instance.footstepSurfaces[index2].surfaceTag == gameObjectTag)
                        {
                            hitSurfaceID = index2;
                            break;
                        }
                }
            }

            if (!flag1)
                return;
            RoundManager.PlayRandomClip(shovelAudio, hitSfx);
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
        RoundManager.PlayRandomClip(shovelAudio, hitSfx);
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