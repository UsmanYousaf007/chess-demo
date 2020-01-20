/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using HUFEXT.GenericGDPR.Runtime.API;

namespace TurboLabz.InstantFramework
{
    public class EarnRewardsView : View
    {

        public Button closeButton;
        public Text titleText;
        public Text inforText;
        public RectTransform rewardBar;
        public Text percentage;

        private float rewardBarOriginalWidth;

        //Signals
        public Signal closeDialogueSignal = new Signal();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public void Init()
        {
            //Set texts
            titleText.text = localizationService.Get(LocalizationKey.EARN_REWARDS_TITLE);
            inforText.text = localizationService.Get(LocalizationKey.EARN_REWARDS_INFO_TEXT);


            //Set Button Listeners
            closeButton.onClick.AddListener(OnCloseDailogue);

            rewardBarOriginalWidth = rewardBar.sizeDelta.x;
        }

        private void OnCloseDailogue()
        {
            audioService.PlayStandardClick();
            closeDialogueSignal.Dispatch();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetupRewardBar()
        {
            var barFillPercentage = playerModel.rewardCurrentPoints / playerModel.rewardPointsRequired;
            rewardBar.sizeDelta = new Vector2(rewardBarOriginalWidth * barFillPercentage, rewardBar.sizeDelta.y);
            percentage.text = Mathf.RoundToInt(barFillPercentage*100) + "%";
        }

    }


}
