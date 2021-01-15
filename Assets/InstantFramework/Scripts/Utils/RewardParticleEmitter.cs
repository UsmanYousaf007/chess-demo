
/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2021 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using DG.Tweening;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class RewardParticleEmitter : View
{
    public GameObject rewardParticlePrefab;
    public int numParticles;
    public GameObject targetObject;

    public float spreadTime;

    public AudioClip spreadSFX;
    public AudioClip travelSFX;

    public Signal<AudioClip> playSFXSignal = new Signal<AudioClip>();

    public void PlayFx(int a_numParticles)
    {
        numParticles = a_numParticles;
        PlayFx();
    }

    public void PlayFx()
    {
        PlaySequence();

        for (int i = 0; i < numParticles; i++)
        {
            GameObject rewardParticle = GameObject.Instantiate(rewardParticlePrefab, gameObject.transform);
            RewardParticle rewardParticleScript = rewardParticle.GetComponent<RewardParticle>();
            rewardParticleScript.targetObject = targetObject;
            rewardParticle.SetActive(true);
        }
    }

    void PlaySequence()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => playSFXSignal.Dispatch(spreadSFX));
        sequence.AppendInterval(spreadTime);
        sequence.AppendCallback(() => playSFXSignal.Dispatch(travelSFX));
        sequence.PlayForward();
    }
}
