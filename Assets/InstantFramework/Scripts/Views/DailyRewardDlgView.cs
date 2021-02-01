/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using System;
using DG.Tweening;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class DailyRewardDlgView : View
    {
        [SerializeField] private RewardUIContainer[] _rewardContainers;

        [SerializeField] private Button _collectBtn;
        [SerializeField] private Button _collect2xBtn;
        public GameObject toolTip;
        [SerializeField] private GameObject _coinsContainer;
        [SerializeField] private Text _coinsText;
        [SerializeField] private GameObject _gemsContainer;
        [SerializeField] private Text _gemsText;
        [SerializeField] private RewardParticleEmitter _gemsFx;
        [SerializeField] private RewardParticleEmitter _coinsFx;

        public Signal _collectBtnClickedSignal = new Signal();
        public Signal _collect2xBtnClickedSignal = new Signal();

        //private Animator _animator;
        private int _playerGems;
        private int _playerCoins;
        private int _gemsRewardQuantity;
        private int _coinsRewardQuantity;

        public void Init()
        {
            for (int i = 0; i < _rewardContainers.Length; i++)
            {
                _rewardContainers[i].containerParent.SetActive(false);
            }

            _collectBtn.onClick.AddListener(() =>
            {
                PlaySimpleCollectAnimation();
                //_collectBtnClickedSignal?.Dispatch();
            });

            _collect2xBtn.onClick.AddListener(() =>
            {
                _collect2xBtnClickedSignal?.Dispatch();
            });

            _coinsContainer.SetActive(false);
            _gemsContainer.SetActive(false);

            toolTip.SetActive(false);

            //_animator = GetComponent<Animator>();
            //_animator.enabled = false;
        }

        public void Show(long coins, long gems)
        {
            _playerCoins = (int)coins;
            _playerGems = (int)gems;

            _coinsContainer.SetActive(false);
            _gemsContainer.SetActive(false);
            _coinsText.text = coins.ToString();
            _gemsText.text = gems.ToString();
            _collectBtn.enabled = true;
            _collect2xBtn.enabled = true;

            //_gemsRewardQuantity = 0;
            //_coinsRewardQuantity = 0;

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            //_animator.enabled = false;
        }

        public void UpdateView(RewardDlgVO vo)
        {
            for (int j = 0; j < _rewardContainers.Length; j++)
            {
                _rewardContainers[j].containerParent.SetActive(false);
            }

            for (int i = 0; i < vo.rewardShortCodes.Count; i++)
            {
                for (int j = 0; j < _rewardContainers.Length; j++)
                {
                    if (_rewardContainers[j].shortCode == vo.rewardShortCodes[i])
                    {
                        int qty = vo.GetRewardItemQty(i);
                        _rewardContainers[j].quantityText.text = "x" + qty.ToString("N0");
                        _rewardContainers[j].containerParent.SetActive(true);

                        if (_rewardContainers[j].shortCode == GSBackendKeys.PlayerDetails.GEMS)
                        {
                            _gemsRewardQuantity = qty;
                        }
                        else if (_rewardContainers[j].shortCode == GSBackendKeys.PlayerDetails.COINS)
                        {
                            _coinsRewardQuantity = qty;
                        }
                    }
                }
            }
        }

        private void PlaySimpleCollectAnimation()
        {
            _collectBtn.enabled = false;
            _collect2xBtn.enabled = false;

            //_animator.enabled = true;
            PlayGemsAnimation();
        }

        #region Animation Events
        private void PlayGemsAnimation()
        {
            _gemsContainer.SetActive(true);
            _gemsFx.PlayFx(_gemsRewardQuantity < 10 ? _gemsRewardQuantity : 10);
            Invoke("PlayGemsCountUpdateAnimation", 1.3f);
        }

        private void PlayGemsCountUpdateAnimation()
        {
            _gemsText.gameObject.SetActive(true);
            iTween.ValueTo(this.gameObject,
                    iTween.Hash(
                        "from", _playerGems,
                        "to", _playerGems + _gemsRewardQuantity,
                        "time", 0.75f,
                        "onupdate", "OnGemCountUpdate",
                        "onupdatetarget", this.gameObject
                        ));

            Invoke("PlayCoinsAnimation", 0.75f);
        }

        private void PlayCoinsAnimation()
        {
            _coinsContainer.SetActive(true);
            _coinsFx.PlayFx(_coinsRewardQuantity < 10 ? _coinsRewardQuantity : 10);
            Invoke("PlayCoinsCountUpdateAnimation", 1.5f);
        }

        private void PlayCoinsCountUpdateAnimation()
        {
            _coinsText.gameObject.SetActive(true);
            iTween.ValueTo(this.gameObject,
                    iTween.Hash(
                        "from", _playerCoins,
                        "to", _playerCoins + _coinsRewardQuantity,
                        "time", 0.75f,
                        "onupdate", "OnCoinsCountUpdate",
                        "onupdatetarget", this.gameObject
                        ));

            Invoke("OnAnimationEnd", 0.75f);
        }

        private void OnAnimationEnd()
        {
            _collectBtnClickedSignal?.Dispatch();
        }
        #endregion

        #region Counting text animation updates
        private void OnGemCountUpdate(int val)
        {
            _gemsText.text = val.ToString("N0");
        }

        private void OnCoinsCountUpdate(int val)
        {
            _coinsText.text = val.ToString("N0");
        }
        #endregion
    }
}