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
    public class RewardDlgV2View : View
    {
        [SerializeField] private RewardUIContainer[] _rewardContainers;
        [SerializeField] private Button _continueButton;

        public Signal ContinueButtonSignal = new Signal();

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

        public void UpdateView(RewardDlgV2VO.Reward reward)
        {
            for (int i = 0; i < _rewardContainers.Length; i++)
            {
                _rewardContainers[i].containerParent.SetActive(false);
                if (_rewardContainers[i].shortCode == reward.ShortCode)
                {
                    _rewardContainers[i].quantityText.text = reward.Quantity.ToString();
                    _rewardContainers[i].containerParent.SetActive(true);
                }
            }
        }
    }
}