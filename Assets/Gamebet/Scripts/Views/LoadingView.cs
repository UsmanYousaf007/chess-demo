/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-11 23:47:01 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine.UI;

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet 
{
    public class LoadingView : View
    {
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        public Text messageLabel;
        
        public void Init()
        {
            messageLabel.text = localizationService.Get(LocalizationKey.LD_MESSAGE_LABEL);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
