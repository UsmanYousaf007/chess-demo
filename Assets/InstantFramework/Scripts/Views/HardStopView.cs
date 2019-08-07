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
using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class HardStopView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public Text errorMessageLabel;
        public Text errorCodeLabel;

        public void Init()
        {
            errorMessageLabel.text = localizationService.Get(LocalizationKey.HARD_STOP);
        }

        public void Show()
        {
            //gameObject.SetActive(true);
        }

        public void SetErrorAndHalt(BackendResult error)
        {
            if(error == BackendResult.GAME_CRASH_SIGNAL)
            {
                string errorCode = Debug.isDebugBuild ? error.ToString() : ((int)error).ToString();
                errorCodeLabel.text = "(Error: " + errorCode + ")";
                errorMessageLabel.text = Truncate(LogUtil.errorString, 500);
                gameObject.SetActive(true);
                StartCoroutine(HaltSystem());
            }
            else
            {
                string errorCode = Debug.isDebugBuild ? error.ToString() : ((int)error).ToString();
                errorCodeLabel.text = "(Error: " + errorCode + ")";

                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    errorCodeLabel.text = localizationService.Get(LocalizationKey.CHECK_INTERNET_CONNECTION);
                }

                //StartCoroutine(HaltSystem());

                LogUtil.Log("!!! Failed !!! " + errorCodeLabel.text, "red");

            }
        }

        public string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        IEnumerator HaltSystem()
        {
            yield return null;

            #if !UNITY_EDITOR
            while (true)
            {
                Thread.Sleep(60000);
            }
            #endif

            LogUtil.Log("!!! System Halted !!! " + errorCodeLabel.text, "red");
        }
    }
}
