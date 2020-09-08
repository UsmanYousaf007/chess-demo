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
using System.Collections;

namespace TurboLabz.InstantFramework
{
    public class UpdateView : View
    {
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAppUpdateService appUpdateService { get; set; }


        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        public Text updateButtonText;
        public Button updateButton;
        public Text updateLabel;
        public Image updateIcon;
        public string updateURL;

        private IRoutineRunner routineRunner;
        private bool hasAppServiceReturnedResult;
        private bool isUpdateAvailable;

        public void Init()
        {
            routineRunner = new NormalRoutineRunner();
            updateLabel.text = localizationService.Get(LocalizationKey.UPDATE_WAIT);
            updateButtonText.text = localizationService.Get(LocalizationKey.UPDATE_BUTTON);
            updateButton.onClick.AddListener(OnUpdateButtonClicked);

            routineRunner.StartCoroutine(CheckAppUpdateFlag());
        }

        public void SetUpdateURL(string url)
        {
            updateURL = url;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetUpdateButton()
        {
            Debug.Log("UPDATEVIEW Show isUpdateAvailable: " + isUpdateAvailable.ToString());
            if (isUpdateAvailable)
            {
                updateLabel.text = localizationService.Get(LocalizationKey.UPDATE);
                updateIcon.gameObject.SetActive(true);
                updateButton.interactable = true;
            }
            else
            {
                updateButton.interactable = false;
            }

            updateButton.gameObject.SetActive(true);
        }

        public void SetAppUpdateFlag(bool isUpdateAvailable)
        {
            this.isUpdateAvailable = isUpdateAvailable;
            hasAppServiceReturnedResult = true;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        void OnUpdateButtonClicked()
        {
#if UNITY_ANDROID
            Application.OpenURL(appInfoModel.androidURL);
#elif UNITY_IOS
            Application.OpenURL(appInfoModel.iosURL);
#else
            LogUtil.Log("UPDATES NOT SUPPORTED ON THIS PLATFORM.", "red");
#endif
        }

        private IEnumerator CheckAppUpdateFlag()
        {
            while (!hasAppServiceReturnedResult)
            {
                yield return new WaitForSeconds(1.0f);
            }
            while (hasAppServiceReturnedResult)
            {
                SetUpdateButton();
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_UPDATE);
                yield break;
            }
        }
    }
}
