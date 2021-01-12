
/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2021 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI.Extensions;
using DG.Tweening;
using UnityEngine.UI;

public class RewardAnimSequence : MonoBehaviour
{
    public GameObject slamCoinFxParent;
    public ParticleSystem slamCoinFx;

    public GameObject RewardCoinFx;
    public RewardParticleEmitter rewardParticleEmitter;

    public ParticleSystem spinGlowGold;
    public ParticleSystem rewardFillFx;

    public Text countRewardText;
    public int countReward;
    private int numOfParticles = 10;

    public void PlayInit()
    {
        rewardFillFx.gameObject.SetActive(false);
        spinGlowGold.gameObject.SetActive(false);
        countRewardText.gameObject.SetActive(false);
        RewardCoinFx.SetActive(false);
    }

    public void SetupRewardQuantity(int a_countReward)
    {
        countReward = a_countReward;
        numOfParticles = countReward > 10 ? 10 : countReward;
    }

    public void PlayFx()
    {
        slamCoinFx.Stop();
        slamCoinFx.gameObject.SetActive(true);
        slamCoinFx.Play();

        spinGlowGold.gameObject.SetActive(true);
        spinGlowGold.Play();
    }

    public void PlayCoinParticles()
    {
        RewardCoinFx.SetActive(true);
        rewardParticleEmitter.PlayFx(numOfParticles);
    }

    public void PlayRewardFillParticles()
    {
        rewardFillFx.gameObject.SetActive(true);
        rewardFillFx.Play();

        countRewardText.text = "x0";
        countRewardText.gameObject.SetActive(true);
        iTween.ValueTo(this.gameObject,
                iTween.Hash(
                    "from", 0,
                    "to", countReward,
                    "time", 0.75f,
                    "onupdate", "OnCountUpdate",
                    "onupdatetarget", this.gameObject
                    //"oncomplete", "AnimationComplete"
                ));
    }

    public void StopRewardFillParticles()
    {
        rewardFillFx.gameObject.SetActive(false);
    }

    public void EndAnimationSequence()
    {
        RewardCoinFx.SetActive(false);

        foreach (Transform child in slamCoinFxParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void OnCountUpdate(int val)
    {
        countRewardText.text = val.ToString("N0");
    }
}
