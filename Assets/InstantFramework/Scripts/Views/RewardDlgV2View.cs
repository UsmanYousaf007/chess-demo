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
using GameAnalyticsSDK;

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
        private int _trophies = 0;

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

        public void UpdateView(RewardDlgV2VO.Reward reward, bool rewardedVideoWatched = false, bool showCoinChest = false, int trophies = 0)
        {
            _reward = reward;
            _showCoinChest = showCoinChest;
            _trophies = trophies;

            _currentChestAnimSequence?.gameObject.SetActive(false);
            _currentChestAnimSequence?.ResetAnimation();
            _currentChestAnimSequence = null;

            for (int i = 0; i < _rewardContainers.Length; i++)
            {
                _rewardContainers[i].containerParent.SetActive(false);
                //_rewardContainers[i].effect.gameObject.SetActive(false);
                Animator animator = _rewardContainers[i].containerParent.GetComponent<Animator>();
                //RewardAnimSequence animSequence = _rewardContainers[i].containerParent.GetComponentInChildren<RewardAnimSequence>(true);
                animator.enabled = false;

                if (_reward != null)
                {
                    if (_rewardContainers[i].shortCode == _reward.ShortCode)
                    {
                        //_rewardContainers[i].quantityText.text = "x0";
                        _currentRewardContainer = _rewardContainers[i];
                        _currentAnimSequence = _rewardContainers[i].containerParent.GetComponentInChildren<RewardAnimSequence>(true);
                    }
                }
                else /// Case for trophies, since trophies are not a part of reward structure and they are separate from rewards.
                {
                    if (_rewardContainers[i].shortCode == "trophies")
                    {
                        //_rewardContainers[i].quantityText.text = "x0";
                        _currentRewardContainer = _rewardContainers[i];
                        _currentAnimSequence = _rewardContainers[i].containerParent.GetComponentInChildren<RewardAnimSequence>(true);
                    }
                }
            }

            _rvReelsPanel.gameObject.SetActive(rewardedVideoWatched);
            _watchingVideoText.gameObject.SetActive(rewardedVideoWatched);
            InvokeStartAnimationCoroutine();
        }

        public IEnumerator StartAnimationCoroutine()
        {
            yield return new WaitForSeconds(0.25f);

            if (_reward != null)
            {
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
            }
            else
            {
                if (_trophies > 0)
                {
                    _currentAnimSequence.SetupRewardQuantity(_trophies);

                    _currentAnimSequence?.transform.parent.gameObject.SetActive(true);

                    _trophies = 0;
                }
            }

            _currentRewardContainer.containerParent.SetActive(true);
        }
    }
}