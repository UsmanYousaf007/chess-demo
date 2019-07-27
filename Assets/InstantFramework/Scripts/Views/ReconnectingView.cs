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

namespace TurboLabz.InstantFramework
{
    public class ReconnectingView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public Text reconnectingLabel;
        public GameObject popUp;
        public GameObject uiBlocker;

        public void Init()
        {
            reconnectingLabel.text = localizationService.Get(LocalizationKey.RECONNECTING);
            uiBlocker.SetActive(false);
            popUp.SetActive(false);
        }

        public void ShowPopUp()
        {
            if (!popUp.activeSelf)
            {
                popUp.SetActive(true);
            }
        }

        public void HidePopUp()
        {
            if (popUp.activeSelf)
            {
                popUp.SetActive(false);
            }
        }
    }
}
