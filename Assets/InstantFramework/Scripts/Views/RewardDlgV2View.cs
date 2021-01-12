/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;
using DG.Tweening;
using System.Collections;
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
        private RewardAnimSequence _currentAnimSequence = null;
        private LobbyChestAnimSequence _currentChestAnimSequence = null;
        private RewardUIContainer _currentRewardContainer = null;
        private bool _showCoinChest = false;

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
            StopCoroutine(StartAnimationCoroutine());

            _currentChestAnimSequence?.gameObject.SetActive(false);
            _currentChestAnimSequence?.ResetAnimation();
            _currentChestAnimSequence = null;
        }

        public void InvokeStartAnimationCoroutine()
        {
            StartCoroutine(StartAnimationCoroutine());
        }

        public void UpdateView(RewardDlgV2VO.Reward reward, bool rewardedVideoWatched = false, bool showCoinChest = false)
        {
            _reward = reward;
            _showCoinChest = showCoinChest;

            _currentChestAnimSequence?.gameObject.SetActive(false);
            _currentChestAnimSequence?.ResetAnimation();
            _currentChestAnimSequence = null;

            for (int i = 0; i < _rewardContainers.Length; i++)
            {
                _rewardContainers[i].containerParent.SetActive(false);
                //_rewardContainers[i].effect.gameObject.SetActive(false);
                Animator animator = _rewardContainers[i].containerParent.GetComponent<Animator>();
                RewardAnimSequence animSequence = _rewardContainers[i].containerParent.GetComponentInChildren<RewardAnimSequence>(true);
                animator.enabled = false;

                if (_rewardContainers[i].shortCode == reward.ShortCode)
                {
                    _rewardContainers[i].quantityText.text = "x0";
                    _currentRewardContainer = _rewardContainers[i];
                    //_rewardContainers[i].containerParent.SetActive(true);
                    _currentAnimSequence = animSequence;
                }
            }

            _rvReelsPanel.gameObject.SetActive(rewardedVideoWatched);
            _watchingVideoText.gameObject.SetActive(rewardedVideoWatched);

            //_continueButton.interactable = false;
        }

        //public void PlayItemAnimation()
        //{
        //    for (int i = 0; i < _rewardContainers.Length; i++)
        //    {
        //        if (_rewardContainers[i].containerParent.activeSelf)
        //        {
        //            _rewardContainers[i].effect.gameObject.SetActive(true);
        //            _rewardContainers[i].effect.Play();

        //            RewardParticleEmitter rewardParticleEmitter = _rewardContainers[i].containerParent.GetComponentInChildren<RewardParticleEmitter>(true);
        //            if (rewardParticleEmitter != null)
        //            {
        //                _currentActiveTextObject = _rewardContainers[i].quantityText;
        //                rewardParticleEmitter.gameObject.SetActive(true);
        //                if (_reward.Quantity < 10)
        //                {
        //                    rewardParticleEmitter.PlayFx(_reward.Quantity);
        //                }
        //                else
        //                {
        //                    rewardParticleEmitter.PlayFx(10);
        //                }
        //            }
        //        }
        //    }

        //    PlayRewardFillParticles();
        //}

        //public void PlayRewardFillParticles()
        //{
        //    _currentActiveTextObject.text = "x0";
        //    iTween.ValueTo(this.gameObject,
        //            iTween.Hash(
        //                "from", 0,
        //                "to", _reward.Quantity,
        //                "time", 0.75f,
        //                "onupdate", "OnCountUpdate",
        //                "onupdatetarget", this.gameObject,
        //                "oncomplete", "AnimationComplete"
        //            ));
        //}

        //private void OnCountUpdate(int val)
        //{
        //    _currentActiveTextObject.text = "x" + val.ToString();
        //}

        //private void AnimationComplete()
        //{
        //    //_continueButton.interactable = true;
        //}

        public IEnumerator StartAnimationCoroutine()
        {
            yield return new WaitForSeconds(0.25f);
            //yield return new WaitForFixedUpdate();

            if (_reward.ShortCode == GSBackendKeys.PlayerDetails.COINS && _showCoinChest)
            {
                _currentChestAnimSequence = _currentRewardContainer.containerParent.GetComponentInChildren<LobbyChestAnimSequence>(true);
                _currentChestAnimSequence.countReward = _reward.Quantity;

                _currentChestAnimSequence?.gameObject.SetActive(true);
                _currentAnimSequence?.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                _currentAnimSequence.SetupRewardQuantity(_reward.Quantity);

                _currentAnimSequence?.transform.parent.gameObject.SetActive(true);
            }

            _currentRewardContainer.containerParent.SetActive(true);
        }
    }
}