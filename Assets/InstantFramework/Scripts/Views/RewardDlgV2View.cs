/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;
using DG.Tweening;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class RewardDlgV2View : View
    {
        [SerializeField] private GameObject _rvReelsPanel;
        [SerializeField] private TextMeshProUGUI _watchingVideoText;
        [SerializeField] private RewardUIContainer[] _rewardContainers;
        [SerializeField] private Button _continueButton;

        public Signal ContinueButtonSignal = new Signal();

        private RewardDlgV2VO.Reward _reward;
        private Text _currentActiveTextObject;

        public void Init()
        {
            _continueButton.onClick.AddListener(() =>
            {
                ContinueButtonSignal.Dispatch();
            });
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateView(RewardDlgV2VO.Reward reward, bool rewardedVideoWatched = false)
        {
            _reward = reward;

            for (int i = 0; i < _rewardContainers.Length; i++)
            {
                _rewardContainers[i].containerParent.SetActive(false);
                if (_rewardContainers[i].shortCode == reward.ShortCode)
                {
                    _rewardContainers[i].quantityText.text = "x0";
                    //_rewardContainers[i].quantityText.text = reward.Quantity.ToString("N0");
                    _rewardContainers[i].containerParent.SetActive(true);
                }
            }

            _rvReelsPanel.gameObject.SetActive(rewardedVideoWatched);
            _watchingVideoText.gameObject.SetActive(rewardedVideoWatched);

            _continueButton.interactable = false;
        }

        public void PlayItemAnimation()
        {
            for (int i = 0; i < _rewardContainers.Length; i++)
            {
                if (_rewardContainers[i].containerParent.activeSelf)
                {
                    _rewardContainers[i].effect.gameObject.SetActive(true);
                    _rewardContainers[i].effect.Play();

                    RewardParticleEmitter rewardParticleEmitter = _rewardContainers[i].containerParent.GetComponentInChildren<RewardParticleEmitter>(true);
                    if (rewardParticleEmitter != null)
                    {
                        _currentActiveTextObject = _rewardContainers[i].quantityText;
                        rewardParticleEmitter.gameObject.SetActive(true);
                        if (_reward.Quantity < 10)
                        {
                            rewardParticleEmitter.PlayFx(_reward.Quantity);
                        }
                        else
                        {
                            rewardParticleEmitter.PlayFx(10);
                        }
                    }
                }
            }

            PlayRewardFillParticles();
        }

        public void PlayRewardFillParticles()
        {
            _currentActiveTextObject.text = "x0";
            iTween.ValueTo(this.gameObject,
                    iTween.Hash(
                        "from", 0,
                        "to", _reward.Quantity,
                        "time", 0.75f,
                        "onupdate", "OnCountUpdate",
                        "onupdatetarget", this.gameObject,
                        "oncomplete", "AnimationComplete"
                    ));
        }

        private void OnCountUpdate(int val)
        {
            _currentActiveTextObject.text = "x" + val.ToString("N0");
        }

        private void AnimationComplete()
        {
            _continueButton.interactable = true;
        }
    }
}