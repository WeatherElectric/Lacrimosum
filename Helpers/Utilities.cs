using System.Linq;

namespace Lacrimosum.Helpers;

public static class Utilities
{
    public static List<PlayerControllerB> allPlayerScripts = [];

    internal static void Init()
    {
        On.StartOfRound.Start += OnStartOfRound;
        RoR2Plugin.ModConsole.LogDebug("StartOfRound.Start hooked!");
    }

    private static void OnStartOfRound(On.StartOfRound.orig_Start orig, StartOfRound self)
    {
        orig(self);
        allPlayerScripts = self.allPlayerScripts.ToList();
    }
        
    public static int RoundToInt(this float value)
    {
        return (int)System.Math.Round(value);
    }
    
    public static void PlayAudibleNoise(AudioSource audioSource, AudioClip noiseSfx, Vector3 sourceLocation, float noiseRange, float noiseLoudness, bool isInElevator)
    {
        WalkieTalkie.TransmitOneShotAudio(audioSource, noiseSfx);
        RoundManager.Instance.PlayAudibleNoise(sourceLocation, noiseRange, noiseLoudness, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
    }

    public static T LoadPersistentAsset<T>(this AssetBundle bundle, string assetPath) where T : Object
    {
        Object asset = bundle.LoadAsset<T>(assetPath);
        
        if (asset == null)
        {
            RoR2Plugin.ModConsole.LogError($"Failed to load asset at path {assetPath}!");
            return null;
        }

        asset.hideFlags = HideFlags.DontUnloadUnusedAsset;
        return (T)asset;
    }

    public static T[] LoadAllPersistentAssets<T>(this AssetBundle bundle) where T : Object
    {
        var assets = bundle.LoadAllAssets<T>();
        
        foreach (var asset in assets)
        {
            asset.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        if (assets.Length != 0) return assets;
        RoR2Plugin.ModConsole.LogError("Failed to load any assets!");
        return null;

    }
    
    public static int GetPlayerIndex(this PlayerControllerB player)
    {
        return allPlayerScripts.IndexOf(player);
    }

    public static void Revive(this PlayerControllerB player)
    {
        
        StartOfRound.Instance.allPlayersDead = false;
        player.ResetPlayerBloodObjects(player.isPlayerDead);
        if (!player.isPlayerDead && !player.isPlayerControlled) return;
        player.isClimbingLadder = false;
        player.clampLooking = false;
        player.inVehicleAnimation = false;
        player.disableMoveInput = false;
        player.ResetZAndXRotation();
        player.thisController.enabled = true;
        player.health = 100;
        player.disableLookInput = false;
        player.disableInteract = false;
        player.isPlayerDead = false;
        player.isPlayerControlled = true;
        player.isInElevator = true;
        player.isInHangarShipRoom = true;
        player.isInsideFactory = false;
        player.parentedToElevatorLastFrame = false;
        player.overrideGameOverSpectatePivot = null;
        StartOfRound.Instance.SetPlayerObjectExtrapolate(enable: false);
        player.TeleportPlayer(StartOfRound.Instance.GetPlayerSpawnPosition(player.GetPlayerIndex()));
        player.setPositionOfDeadPlayer = false;
        player.DisablePlayerModel(player.gameObject, true, true);
        player.helmetLight.enabled = false;
        player.Crouch(false);
        player.criticallyInjured = false;
        player.playerBodyAnimator.SetBool("Limp", false);
        player.bleedingHeavily = false;
        player.activatingItem = false;
        player.twoHanded = false;
        player.inShockingMinigame = false;
        player.inSpecialInteractAnimation = false;
        player.freeRotationInInteractAnimation = false;
        player.disableSyncInAnimation = false;
        player.inAnimationWithEnemy = null;
        player.holdingWalkieTalkie = false;
        player.speakingToWalkieTalkie = false;
        player.isSinking = false;
        player.isUnderwater = false;
        player.sinkingValue = 0f;
        player.statusEffectAudio.Stop();
        player.DisableJetpackControlsLocally();
        player.mapRadarDotAnimator.SetBool("dead", false);
        player.externalForceAutoFade = Vector3.zero;
        if (player.IsOwner)
        {
            HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", false);
            player.hasBegunSpectating = false;
            HUDManager.Instance.RemoveSpectateUI();
            HUDManager.Instance.gameOverAnimator.SetTrigger("revive");
            player.hinderedMultiplier = 1f;
            player.isMovementHindered = 0;
            player.sourcesCausingSinking = 0;
            player.reverbPreset = StartOfRound.Instance.shipReverb;
        }
        SoundManager.Instance.earsRingingTimer = 0f;
        player.voiceMuffledByEnemy = false;
        if (player.currentVoiceChatIngameSettings != null)
        {
            if (player.currentVoiceChatIngameSettings.voiceAudio == null)
            {
                player.currentVoiceChatIngameSettings.InitializeComponents();
            }
            if (player.currentVoiceChatIngameSettings.voiceAudio == null)
            {
                return;
            }
            player.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
        }
        HUDManager.Instance.UpdateHealthUI(100, hurtPlayer: false);
        player.spectatedPlayerScript = null;
        HUDManager.Instance.audioListenerLowPass.enabled = false;
        StartOfRound.Instance.SetSpectateCameraToGameOverMode(enableGameOver: false, player);
        var ragdolls = Object.FindObjectsOfType<RagdollGrabbableObject>();
        foreach (var ragdoll in ragdolls)
        {
            if (!StartOfRound.Instance.IsServer || ragdoll.ragdoll.playerScript != player) continue;
            if (ragdoll.NetworkObject.IsSpawned) ragdoll.NetworkObject.Despawn();
            else Object.Destroy(ragdoll.gameObject);
        }
        StartOfRound.Instance.livingPlayers += 1;
        StartOfRound.Instance.shipAnimator.ResetTrigger("ShipLeave");
    }
    
    public static void CreateExplosion(Vector3 explosionPosition, bool spawnExplosionEffect = false, int damage = 20,
        float minDamageRange = 0f, float maxDamageRange = 1f, int enemyHitForce = 6,
        CauseOfDeath causeOfDeath = CauseOfDeath.Blast, PlayerControllerB attacker = null)
    {
        Transform holder = null;

        if (RoundManager.Instance != null && RoundManager.Instance.mapPropsContainer != null &&
            RoundManager.Instance.mapPropsContainer.transform != null)
            holder = RoundManager.Instance.mapPropsContainer.transform;

        if (spawnExplosionEffect)
            Object.Instantiate(StartOfRound.Instance.explosionPrefab, explosionPosition, Quaternion.Euler(-90f, 0f, 0f),
                holder).SetActive(true);

        var num = Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position,
            explosionPosition);
        switch (num)
        {
            case < 14f:
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
                break;
            case < 25f:
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
                break;
        }

        // ReSharper disable once Unity.PreferNonAllocApi
        var array = Physics.OverlapSphere(explosionPosition, maxDamageRange, 2621448, QueryTriggerInteraction.Collide);
        foreach (var t in array)
        {
            var num2 = Vector3.Distance(explosionPosition, t.transform.position);
            if (num2 > 4f && Physics.Linecast(explosionPosition, t.transform.position + Vector3.up * 0.3f, 256,
                    QueryTriggerInteraction.Ignore)) continue;

            switch (t.gameObject.layer)
            {
                case 3:
                {
                    var playerControllerB = t.gameObject.GetComponent<PlayerControllerB>();
                    if (playerControllerB != null && playerControllerB.IsOwner)
                    {
                        var damageMultiplier =
                            1f - Mathf.Clamp01((num2 - minDamageRange) / (maxDamageRange - minDamageRange));

                        playerControllerB.DamagePlayer((int)(damage * damageMultiplier), causeOfDeath: causeOfDeath);
                    }

                    break;
                }
                case 21:
                {
                    var componentInChildren = t.gameObject.GetComponentInChildren<Landmine>();
                    if (componentInChildren != null && !componentInChildren.hasExploded && num2 < 6f)
                    {
                        Debug.Log("Setting off other mine");
                        componentInChildren.StartCoroutine(
                            componentInChildren.TriggerOtherMineDelayed(componentInChildren));
                    }

                    break;
                }
                case 19:
                {
                    var componentInChildren2 = t.gameObject.GetComponentInChildren<EnemyAICollisionDetect>();
                    if (componentInChildren2 != null && componentInChildren2.mainScript.IsOwner && num2 < 4.5f)
                        componentInChildren2.mainScript.HitEnemyOnLocalClient(enemyHitForce, playerWhoHit: attacker);
                    break;
                }
            }
        }

        var num3 = ~LayerMask.GetMask("Colliders");
        // ReSharper disable once Unity.PreferNonAllocApi
        array = Physics.OverlapSphere(explosionPosition, 10f, num3);
        foreach (var t in array)
        {
            var component = t.GetComponent<Rigidbody>();
            if (component != null) component.AddExplosionForce(70f, explosionPosition, 10f);
        }
    }
}