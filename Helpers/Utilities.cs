namespace Lacrimosum.Helpers;

public static class Utilities
{
    public static int RoundToInt(this float value)
    {
        return (int)System.Math.Round(value);
    }
    
    public static void PlayAudibleNoise(AudioSource audioSource, AudioClip noiseSfx, Vector3 sourceLocation, float noiseRange, float noiseLoudness, bool isInElevator)
    {
        WalkieTalkie.TransmitOneShotAudio(audioSource, noiseSfx);
        RoundManager.Instance.PlayAudibleNoise(sourceLocation, noiseRange, noiseLoudness, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
    }
    
    public static GameObject GetMushroomWard(this PlayerControllerB player)
    {
        return player.transform.Find("MushroomWard").gameObject;
    }

    public static GameObject GetSaferSpacesBubble(this PlayerControllerB player)
    {
        return player.transform.Find("SaferSpacesBubble").gameObject;
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