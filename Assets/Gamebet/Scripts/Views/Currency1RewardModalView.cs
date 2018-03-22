/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-09 14:50:40 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;

namespace TurboLabz.Gamebet
{
    public class Currency1RewardModalView : View
    {
        public GameObject rewardGroup;

        public Text currency1Label;
        public Text currency2Label;
        public Text currency1RewardLabel;

        public Button claimRewardButton;
        public Text claimRewardButtonLabel;

        public GameObject waitingForRewardGroup;
        public Text waitMessageLabel;

        // View signals.
        public Signal claimRewardButtonClickedSignal = new Signal();

        public void Init()
        {
            claimRewardButton.onClick.AddListener(OnClaimRewardButtonClicked);
        }

        public void UpdateView(Currency1RewardModalVO vo)
        {
            rewardGroup.SetActive(true);
            waitingForRewardGroup.SetActive(false);

            currency1Label.text = vo.currency1.ToString("N0");
            currency2Label.text = vo.currency2.ToString("N0");
            currency1RewardLabel.text = "+" + vo.currency1Reward.ToString("N0");

            claimRewardButtonLabel.text = "CLAIM REWARD";
        }

        public void ShowWaitForReward()
        {
            rewardGroup.SetActive(false);
            waitingForRewardGroup.SetActive(true);

            waitMessageLabel.text = "Getting your reward...";
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnClaimRewardButtonClicked()
        {
            claimRewardButtonClickedSignal.Dispatch();
        }
    }
}
