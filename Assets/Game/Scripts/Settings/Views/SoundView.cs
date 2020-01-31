/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class SoundView : View
    {

        //Sound
        public Button audioOffButton;
        public Button audioOnButton;
        public Text soundText;
        public Text soundEffectsText;
        public Text soundOnText;
        public Text soundOffText;

        //Injections
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        public void Init()
        {
            //Set texts
         
            soundText.text = localizationService.Get(LocalizationKey.SETTINGS_SOUND_TITLE);
            soundEffectsText.text = localizationService.Get(LocalizationKey.SETTINGS_SOUND_EFFECT);
            soundOnText.text = localizationService.Get(LocalizationKey.SETTINGS_ON);
            soundOffText.text = localizationService.Get(LocalizationKey.SETTINGS_OFF);

            //Set Button Listeners
            audioOffButton.onClick.AddListener(OnAudioOffButtonClicked);
            audioOnButton.onClick.AddListener(OnAudioOnButtonClicked);

            RefreshAudioButtons();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RefreshAudioButtons();
        }

        private void OnAudioOffButtonClicked()
        {
            audioService.ToggleAudio(true);
            audioService.PlayStandardClick();
            RefreshAudioButtons();
        }

        private void OnAudioOnButtonClicked()
        {
            audioService.ToggleAudio(false);
            RefreshAudioButtons();
        }

        private void RefreshAudioButtons()
        {
            audioOffButton.gameObject.SetActive(!audioService.IsAudioOn());
            audioOnButton.gameObject.SetActive(audioService.IsAudioOn());
        }
    }


}
