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
        [Serializable]
        public class RewardContainer
        {
            public string shortCode;
            public GameObject containerParent;

            public Text quantityText;
        }

        [SerializeField] private RewardContainer [] _rewardContainers;
        
        public Button _collectBtn;
        public Button _collect2xBtn;

        public Signal<string> _collectBtnClickedSignal = new Signal<string>();
        public Signal<string> _collect2xBtnClickedSignal = new Signal<string>();

        private string messageId;

        public void Init()
        {
            for (int i = 0; i < _rewardContainers.Length; i++)
            {
                _rewardContainers[i].containerParent.SetActive(false);
            }

            _collectBtn.onClick.AddListener(() =>
            {
                _collectBtnClickedSignal?.Dispatch(messageId);
            });

            _collect2xBtn.onClick.AddListener(() =>
            {
                _collect2xBtnClickedSignal?.Dispatch(messageId);
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

        public void UpdateView(RewardDlgVO vo)
        {
            messageId = vo.msgId;

            for (int i = 0; i < vo.rewardShortCodes.Count; i++)
            {
                for (int j = 0; j < _rewardContainers.Length; j++)
                {
                    _rewardContainers[j].containerParent.SetActive(false);

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