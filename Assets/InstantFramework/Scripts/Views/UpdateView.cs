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
            Application.OpenURL("market://details?id=com.turbolabz.instantchess.android.googleplay");
            #elif UNITY_IOS
            Application.OpenURL("https://itunes.apple.com/us/app/chess/id1386718098?mt=8");
            #else
            LogUtil.Log("UPDATES NOT SUPPORTED ON THIS PLATFORM.", "red");
            #endif
        }
    }
}
