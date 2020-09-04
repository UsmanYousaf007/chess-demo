/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;
using TurboLabz.InstantFramework;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace TurboLabz.InstantGame
{
    public class ChestInfoDialogView : View
    {
        [Header("Main Dialogue")]
        public TMP_Text titleText;
        public TMP_Text descText;

        public Button okayButton;
        public TMP_Text okayButtonText;

        public TMP_Text gemsCount;
        public TMP_Text hintsCount;
        public TMP_Text ratingBoostersCount;

        public TMP_Text gemsCount2;
        public TMP_Text hintsCount2;

        public Image chestImage;

        public GameObject middleSectionWithTwoItems;
        public GameObject middleSectionWithThreeItems;

        //Signals
        public Signal closeSignal = new Signal();

        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {

            titleText.text = localizationService.Get(LocalizationKey.TOURNAMENT_CHEST_CONTENT_DIALOGUE_TITLE);
            descText.text = localizationService.Get(LocalizationKey.TOURNAMENT_CHEST_CONTENT_DIALOGUE_DESCRIPTION);
            okayButtonText.text = localizationService.Get(LocalizationKey.TOURNAMENT_CHEST_CONTENT_DIALOGUE_OKAY_BUTTON_TEXT);

            okayButton.onClick.AddListener(OnCloseButtonClicked);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCloseButtonClicked()
        {
            closeSignal.Dispatch();
        }

        public void UpdateView(TournamentReward reward)
        {
            ChestIconsContainer container = ChestIconsContainer.Load();
            chestImage.sprite = container.GetChest(reward.chestType);

            if (reward.chestType == TournamentConstants.ChestType.COMMON) titleText.text = "Arena Common Chest";
            if (reward.chestType == TournamentConstants.ChestType.EPIC) titleText.text = "Arena Epic Chest";
            if (reward.chestType == TournamentConstants.ChestType.RARE) titleText.text = "Arena Rare Chest";

            if (reward.ratingBoosters > 0)
            {
                gemsCount.text = "x" + reward.gems.ToString();
                hintsCount.text = "x" + reward.hints.ToString();
                ratingBoostersCount.text = "x" + reward.ratingBoosters.ToString();
                middleSectionWithTwoItems.SetActive(false);
                middleSectionWithThreeItems.SetActive(true);
            }
            else
            {
                gemsCount2.text = "x" + reward.gems.ToString();
                hintsCount2.text = "x" + reward.hints.ToString();
                middleSectionWithTwoItems.SetActive(true);
                middleSectionWithThreeItems.SetActive(false);
            }
        }
    }
}