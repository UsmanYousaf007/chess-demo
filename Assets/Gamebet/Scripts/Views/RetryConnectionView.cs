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
using strange.extensions.signal.impl;
using TurboLabz.Common;
using UnityEngine;
using System;

namespace TurboLabz.Gamebet
{
    public class RetryConnectionView : View
    {
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        public Text errorMessageLabel;
        public Button retryConnectionButton;

        // Dispatch signals
        public Signal retryConnectionButtonClickedSignal = new Signal();

        public void Init()
        {
            retryConnectionButton.onClick.AddListener(OnRetryConnectionButtonClicked);
        }

        public void SetErrorMessage(BackendResult message)
        {
            int errorCode = (int)(Convert.ChangeType(message, message.GetTypeCode()));

            if (Debug.isDebugBuild)
            {
                errorMessageLabel.text = localizationService.Get(LocalizationKey.RC_ERROR_CODE)  + message + "\nCode No: " + errorCode;
            }
            else
            {
                errorMessageLabel.text = localizationService.Get(LocalizationKey.RC_ERROR_CODE)  + errorCode;
            }

            LogUtil.Log("The error message is : " + message + " error code: " + errorCode, "red");
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnRetryConnectionButtonClicked()
        {
            retryConnectionButtonClickedSignal.Dispatch();
        }
    }
}
