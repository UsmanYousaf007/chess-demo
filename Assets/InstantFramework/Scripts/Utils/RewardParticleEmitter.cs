
/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2021 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

public class RewardParticleEmitter : MonoBehaviour
{
    public GameObject rewardParticlePrefab;
    public int numParticles;
    public GameObject targetObject;

    public void PlayFx()
    {
        for (int i = 0; i < numParticles; i++)
        {
            GameObject rewardParticle = GameObject.Instantiate(rewardParticlePrefab, gameObject.transform);
            RewardParticle rewardParticleScript = rewardParticle.GetComponent<RewardParticle>();
            rewardParticleScript.targetObject = targetObject;
            rewardParticle.SetActive(true);
        }
    }
}
