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

        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IAppUpdatesService appUpdatesService { get; set; }

        // Dispatch Signals
        [Inject] public GetInitDataCompleteSignal getInitDataCompleteSignal { get; set; }

        public Text updateLabel;
        public Text updateButtonText;
        public Text updateLaterButtonText;
        public Button updateButton;
        public Button updateLaterButton;
        public string updateURL;

        public void Init()
        {
            updateLabel.text = localizationService.Get(LocalizationKey.UPDATE);
            updateButtonText.text = localizationService.Get(LocalizationKey.UPDATE_BUTTON);
            updateLaterButtonText.text = localizationService.Get(LocalizationKey.UPDATE_LATER_BUTTON);

            updateButton.onClick.AddListener(OnUpdateButtonClicked);
            updateLaterButton.onClick.AddListener(OnUpdateLaterButtonClicked);
        }

        public void SetUpdateURL(string url)
        {
            updateURL = url;
        }


        public void Show()
        {
            updateLabel.text = settingsModel.updateMessage;

            if (appInfoModel.isMandatoryUpdate)
            {
                updateButton.gameObject.SetActive(true);
                updateLaterButton.gameObject.SetActive(false);
            }
            else
            {
                updateButton.gameObject.SetActive(true);
                updateLaterButton.gameObject.SetActive(true);
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        void OnUpdateLaterButtonClicked()
        {
            appUpdatesService.updateLater = true;
            appUpdatesService.Terminate();
            getInitDataCompleteSignal.Dispatch();
        }

        void OnUpdateButtonClicked()
        {
            // TODO: Update this entire view to support multiple platforms

#if UNITY_ANDROID || UNITY_IOS
            inAppUpdatesService.GoToStore(appInfoModel.storeURL);
#else
            LogUtil.Log("UPDATES NOT SUPPORTED ON THIS PLATFORM.", "red");
#endif
        }
    }
}
