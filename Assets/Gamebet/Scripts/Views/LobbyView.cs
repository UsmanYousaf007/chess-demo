/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class LobbyView : View
    {
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        // Sprite cache is taken from the SpriteCache gameobject in the scene
        public SpriteCache spriteCache;

        public Text currency1;
        public Button currency1BuyButton;

        public Text currency2;
        public Button currency2BuyButton;

        public Button settingsButton;

        public Button playButton;
        public Text playButtonLabel;

        public Button CPUButton;
        public Text CPUButtonLabel;

        public Button learnButton;
        public Text learnButtonLabel;

        public Button freeCurrency1Button;
        public Text freeCurrency1ButtonLabel;

        public Button feedbackButton;
        public Text feedbackButtonLabel;

        public Button profileButton;
        public Image profilePicture;
        public Image profilePictureBorder;
        public Text displayNameLabel;
        public Text levelLabel;
        public Slider xpBar;
        public Image leagueBadge;

        public Button shopButton;

        // View signals
        public Signal currency1BuyButtonClickedSignal = new Signal();
        public Signal currency2BuyButtonClickedSignal = new Signal();
        public Signal settingButtonClickedSignal = new Signal();
        public Signal playButtonClickedSignal = new Signal();
        public Signal CPUButtonClickedSignal = new Signal();
        public Signal learnButtonClickedSignal = new Signal();
        public Signal feedbackButtonClickedSignal = new Signal();
        public Signal freeCurrency1ButtonClickedSignal = new Signal();
        public Signal profileButtonClickedSignal = new Signal();
        public Signal shopButtonClickedSignal = new Signal();

        public void Init()
        {
            currency1BuyButton.onClick.AddListener(OnCurrency1BuyButtonClicked);
            currency2BuyButton.onClick.AddListener(OnCurrency2BuyButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            playButton.onClick.AddListener(OnPlayButtonClicked);
            CPUButton.onClick.AddListener(OnCPUButtonClicked);
            learnButton.onClick.AddListener(OnLearnButtonClicked);
            freeCurrency1Button.onClick.AddListener(OnFreeCurrency1ButtonClicked);
            feedbackButton.onClick.AddListener(OnFeedbackButtonClicked);
            profileButton.onClick.AddListener(OnProfileButtonClicked);
            shopButton.onClick.AddListener(OnShopButtonClicked);
        }

        public void UpdateView(LobbyVO vo)
        {
            currency1.text = vo.currency1.ToString("N0");
            currency2.text = vo.currency2.ToString("N0");

            playButtonLabel.text = localizationService.Get(LocalizationKey.LOBBY_PLAY_BUTTON_LABEL);
            CPUButtonLabel.text = localizationService.Get(LocalizationKey.CPU_BUTTON_LABEL);
            learnButtonLabel.text = localizationService.Get(LocalizationKey.LEARN_BUTTON_LABEL);
            freeCurrency1ButtonLabel.text = localizationService.Get(LocalizationKey.LOBBY_FREE_CURRENCY_1_BUTTON_LABEL);
            feedbackButtonLabel.text = localizationService.Get(LocalizationKey.LOBBY_FEEDBACK_BUTTON_LABEL);

            // Updating profile picture here to display cached profile picture
            // right away.
            UpdateProfilePicture(vo.playerModel.profilePicture);
            UpdateProfilePictureBorder(vo.playerModel.profilePictureBorder);

            levelLabel.text = localizationService.Get(LocalizationKey.LOBBY_LEVEL, vo.level);

            // If the player has reached the maximum level then we don't display
            // the XP. Instead we notify of the max level and show the XP bar as
            // full.
            if (vo.hasReachedMaxLevel)
            {
                xpBar.value = 1f;
            }
            else
            {
                // Normalize xp to start from zero for the level to display on
                // player profile.
                // Add 1 to levelEndXp because we need to display the starting
                // XP of the next level as the ending XP of the current level.
                int levelEndXp = (vo.levelEndXp - vo.levelStartXp) + 1;
                int xp = vo.xp - vo.levelStartXp;

                xpBar.value = (float)xp / levelEndXp;
            }

            displayNameLabel.text = vo.displayName;
            leagueBadge.sprite = spriteCache.GetLeagueBadge(vo.leagueId);
        }

        public void UpdateProfilePicture(Sprite sprite)
        {
            profilePicture.sprite = sprite;
            profilePicture.gameObject.SetActive(sprite != null);
        }

        public void UpdateProfilePictureBorder(Sprite sprite)
        {
            profilePictureBorder.sprite = sprite;
            profilePictureBorder.gameObject.SetActive(sprite != null);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnSettingsButtonClicked()
        {
            settingButtonClickedSignal.Dispatch();
        }

        private void OnCurrency1BuyButtonClicked()
        {
            currency1BuyButtonClickedSignal.Dispatch();
        }

        private void OnCurrency2BuyButtonClicked()
        {
            currency2BuyButtonClickedSignal.Dispatch();
        }

        private void OnFreeCurrency1ButtonClicked()
        {
            freeCurrency1ButtonClickedSignal.Dispatch();
        }

        private void OnPlayButtonClicked()
        {
            playButtonClickedSignal.Dispatch();
        }

        private void OnCPUButtonClicked()
        {
            CPUButtonClickedSignal.Dispatch();
        }

        private void OnLearnButtonClicked()
        {
            learnButtonClickedSignal.Dispatch();
        }

        private void OnFeedbackButtonClicked()
        {
            feedbackButtonClickedSignal.Dispatch();
        }

        private void OnProfileButtonClicked()
        {
            profileButtonClickedSignal.Dispatch();
        }

        private void OnShopButtonClicked()
        {
            shopButtonClickedSignal.Dispatch();
        }
    }
}
