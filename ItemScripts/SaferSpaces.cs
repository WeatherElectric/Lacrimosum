using System;

namespace Lacrimosum.ItemScripts;

[AddComponentMenu("Lacrimosum/Safer Spaces")]
public class SaferSpaces : ItemBehaviour
{
    public static readonly List<SaferSpaces> ActiveSpaces = [];
    
    [Space(10f)]
    [Header("Safer Spaces Settings")]
    [Tooltip("The cooldown between each activation.")]
    public float cooldown = 3f;
    [Tooltip("A list of sounds to pick from when activated.")]
    public AudioClip[] activationSounds;
    [Tooltip("The sound to play when the cooldown ends.")]
    public AudioClip cooldownEndSound;

    [NonSerialized] public bool CanBlock = true;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        cooldown = RoR2Plugin.ModConfig.SaferSpacesCooldown;
    }
    
    public override void GrabItem()
    {
        base.GrabItem();
        ActiveSpaces.Add(this);
        StartCooldown();
    }
    
    public bool IsPlayerHolding(PlayerControllerB player)
    {
        return player == playerHeldBy;
    }

    public void PlaySound()
    {
        PlaySoundServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void PlaySoundServerRpc()
    {
        var randomIndex = UnityEngine.Random.Range(0, activationSounds.Length);
        PlaySoundClientRpc(randomIndex);
    }
    
    [ClientRpc]
    private void PlaySoundClientRpc(int index)
    {
        WalkieTalkie.TransmitOneShotAudio(AudioSource, activationSounds[index]);
        AudioSource.PlayOneShot(activationSounds[index]);
    }

    public void StartCooldown()
    {
        CooldownServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void CooldownServerRpc()
    {
        CooldownClientRpc();
    }
    
    [ClientRpc]
    private void CooldownClientRpc()
    {
        StartCoroutine(Cooldown());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator Cooldown()
    {
        playerHeldBy.GetSaferSpacesBubble().SetActive(false);
        CanBlock = false;
        yield return new WaitForSeconds(cooldown);
        playerHeldBy.GetSaferSpacesBubble().SetActive(true);
        CanBlock = true;
        AudioSource.PlayOneShot(cooldownEndSound);
    }
    
    public override void DiscardItem()
    {
        base.DiscardItem();
        ActiveSpaces.Remove(this);
        LastPlayerHeldBy.GetSaferSpacesBubble().SetActive(false);
        CanBlock = false;
    }
}