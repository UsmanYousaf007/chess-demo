
/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2021 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI.Extensions;
using DG.Tweening;
using UnityEngine.UI;

public class LobbyChestAnimSequence : MonoBehaviour
{
    public ParticleSystem preOpenFx;
    public ParticleSystem OpenFx;

    public GameObject RewardCoinFx;
    public RewardParticleEmitter rewardParticleEmitter;

    public Text countRewardText;
    public int countReward;

    public void LobbyChestAnimSequence_PlayInit()
    {
        preOpenFx.gameObject.SetActive(false);
        OpenFx.gameObject.SetActive(false);
        countRewardText.gameObject.SetActive(false);
        RewardCoinFx.SetActive(false);
    }

    public void LobbyChestAnimSequence_PlayPreOpenFx()
    {
        preOpenFx.gameObject.SetActive(true);
    }

    public void LobbyChestAnimSequence_PlayCoinsFx()
    {
        RewardCoinFx.SetActive(true);
    }

    public void LobbyChestAnimSequence_PlayOpenFx()
    {
        OpenFx.gameObject.SetActive(true);
    }

    public void LobbyChestAnimSequence_End()
    {
        preOpenFx.gameObject.SetActive(false);
        OpenFx.gameObject.SetActive(false);
        countRewardText.gameObject.SetActive(false);
        RewardCoinFx.SetActive(false);
    }
    /*
        public void PlayFx()
        {
            //slamCoinFx.Stop();
            //slamCoinFx.gameObject.SetActive(true);
            //slamCoinFx.Play();

            //spinGlowGold.gameObject.SetActive(true);
            spinGlowGold.Play();
        }

        public void PlayCoinParticles()
        {
            RewardCoinFx.SetActive(true);
            rewardParticleEmitter.PlayFx();
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

        */
}
