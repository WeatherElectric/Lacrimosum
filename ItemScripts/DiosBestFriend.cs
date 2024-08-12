// ReSharper disable Unity.PreferAddressByIdToGraphicsParams
namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Dios' Best Friend")]
public class DiosBestFriend : ItemBehaviour
{
    public static readonly List<DiosBestFriend> ActiveExtraLives = [];
    public PlayerControllerB targetPlayer;
    public PlayerControllerB playerToRevive;

    public override void GrabItem()
    {
        base.GrabItem();
        targetPlayer = playerHeldBy;
        ActiveExtraLives.Add(this);
    }

    public void CheckPlayerDeath(PlayerControllerB player)
    {
        if (targetPlayer != player) return;

        playerToRevive = player;
        ReviveServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReviveServerRpc()
    {
        ReviveClientRpc();
    }

    [ClientRpc]
    public void ReviveClientRpc()
    {
        Revive(playerToRevive);
        RoR2Plugin.ModConsole.LogDebug($"Revived {playerToRevive.playerUsername}");
        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        playerHeldBy.DropAllHeldItems();
        yield return new WaitForSeconds(1f);
        NetworkObject.Despawn();
    }

    public override void DiscardItem()
    {
        base.DiscardItem();
        if (!lastPlayerHeldBy.isPlayerDead) ActiveExtraLives.Remove(this);
        if (!lastPlayerHeldBy.isPlayerDead) targetPlayer = null;
    }

    private void Revive(PlayerControllerB player)
    {
        var playerIndex = player.GetPlayerIndex();
        player.ResetPlayerBloodObjects(player.isPlayerDead);
        player.isClimbingLadder = false;
        player.ResetZAndXRotation();
        player.thisController.enabled = true;
        player.health = 100;
        player.disableLookInput = false;
        if (player.isPlayerDead)
        {
            player.isPlayerDead = false;
            player.isPlayerControlled = true;
            player.isInElevator = true;
            player.isInHangarShipRoom = true;
            player.isInsideFactory = false;
            player.parentedToElevatorLastFrame = false;
            StartOfRound.Instance.SetPlayerObjectExtrapolate(false);
            if (RoR2Plugin.Config.DiosBestFriendRespawnAtShip) player.TeleportPlayer(player.GetSpawnPosition());
            player.setPositionOfDeadPlayer = false;
            player.DisablePlayerModel(StartOfRound.Instance.allPlayerObjects[playerIndex], true, true);
            player.helmetLight.enabled = false;
            player.Crouch(false);
            player.criticallyInjured = false;
            if (player.playerBodyAnimator != null) player.playerBodyAnimator.SetBool("Limp", false);
            player.bleedingHeavily = false;
            player.activatingItem = false;
            player.twoHanded = false;
            player.inSpecialInteractAnimation = false;
            player.disableSyncInAnimation = false;
            player.inAnimationWithEnemy = null;
            player.holdingWalkieTalkie = false;
            player.speakingToWalkieTalkie = false;
            player.isSinking = false;
            player.isUnderwater = false;
            player.sinkingValue = 0f;
            player.statusEffectAudio.Stop();
            player.DisableJetpackControlsLocally();
            player.health = 100;
            player.mapRadarDotAnimator.SetBool("dead", false);
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
        SoundManager.Instance.playerVoicePitchTargets[playerIndex] = 1f;
        SoundManager.Instance.SetPlayerPitch(1f, playerIndex);
        if (player.currentVoiceChatIngameSettings == null) StartOfRound.Instance.RefreshPlayerVoicePlaybackObjects();
        if (player.currentVoiceChatIngameSettings != null)
        {
            if (player.currentVoiceChatIngameSettings.voiceAudio == null)
                player.currentVoiceChatIngameSettings.InitializeComponents();
            if (player.currentVoiceChatIngameSettings.voiceAudio == null) return;
            player.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
        }
        var playerControllerB = GameNetworkManager.Instance.localPlayerController;
        playerControllerB.bleedingHeavily = false;
        playerControllerB.criticallyInjured = false;
        playerControllerB.playerBodyAnimator.SetBool("Limp", false);
        playerControllerB.health = 100;
        HUDManager.Instance.UpdateHealthUI(100, false);
        playerControllerB.spectatedPlayerScript = null;
        HUDManager.Instance.audioListenerLowPass.enabled = false;
        StartOfRound.Instance.SetSpectateCameraToGameOverMode(false, playerControllerB);
        var array = FindObjectsOfType<RagdollGrabbableObject>();
        foreach (var t in array)
        {
            switch (t.isHeld)
            {
                case false:
                {
                    if (IsServer)
                    {
                        if (t.NetworkObject.IsSpawned)
                            t.NetworkObject.Despawn();
                        else
                            Destroy(t.gameObject);
                    }

                    break;
                }
                case true when t.playerHeldBy != null:
                    t.playerHeldBy.DropAllHeldItems();
                    break;
            }
        }
        var array2 = FindObjectsOfType<DeadBodyInfo>();
        foreach (var t in array2)
            Destroy(t.gameObject);
        StartOfRound.Instance.livingPlayers++;
        StartOfRound.Instance.allPlayersDead = false;
        StartOfRound.Instance.UpdatePlayerVoiceEffects();
    }
}