/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 18:54:42 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine.UI;

using strange.extensions.mediation.impl;
using UnityEngine;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class UpdateView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        public Text updateLabel;
        public Text updateButtonText;
        public Button updateButton;
        public string updateURL;

        public void Init()
        {
            updateLabel.text = localizationService.Get(LocalizationKey.UPDATE);
            updateButtonText.text = localizationService.Get(LocalizationKey.UPDATE_BUTTON);

            updateButton.onClick.AddListener(OnUpdateButtonClicked);
        }

        public void SetUpdateURL(string url)
        {
            updateURL = url;
        }


        public void Show()
        {
            updateLabel.text = settingsModel.updateMessage;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        void OnUpdateButtonClicked()
        {
            // TODO: Update this entire view to support multiple platforms

            #if UNITY_ANDROID
            Application.OpenURL(appInfoModel.androidURL);
            #elif UNITY_IOS
            Application.OpenURL(appInfoModel.iosURL);
            #else
            LogUtil.Log("UPDATES NOT SUPPORTED ON THIS PLATFORM.", "red");
            #endif
        }
    }
}
