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
        
        public Button _collectBtn;
        public Button _collect2xBtn;
        public GameObject toolTip;

        public Signal _collectBtnClickedSignal = new Signal();
        public Signal _collect2xBtnClickedSignal = new Signal();

        public void Init()
        {
            for (int i = 0; i < _rewardContainers.Length; i++)
            {
                _rewardContainers[i].containerParent.SetActive(false);
            }

            _collectBtn.onClick.AddListener(() =>
            {
                _collectBtnClickedSignal?.Dispatch();
            });

            _collect2xBtn.onClick.AddListener(() =>
            {
                _collect2xBtnClickedSignal?.Dispatch();
            });

            toolTip.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
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
                        _rewardContainers[j].quantityText.text = $"x{qty}";
                        _rewardContainers[j].containerParent.SetActive(true);
                    }
                }
            }
        }
    }
}