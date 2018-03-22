/// @license #LICENSE# <#LICENSE_URL#>
/// @copyright Copyright (C) #COMPANY# #YEAR# - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author #AUTHOR# <#AUTHOR_EMAIL#>
/// @company #COMPANY# <#COMPANY_URL#>
/// @date #DATE#
/// 
/// @description
/// #DESCRIPTION#

using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;

namespace TurboLabz.Gamebet
{
    public class UpdateAppView : View
    {
        public Text titleLabel;

        public Button updateButton;
        public Text updateButtonLabel;

        public Signal updateButtonClickedSignal = new Signal();

        public void Init()
        {
            updateButton.onClick.AddListener(OnUpdateButtonClicked);

            titleLabel.text = "A new version of game is available";
            updateButtonLabel.text = "UPDATE NOW";
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnUpdateButtonClicked()
        {
            updateButtonClickedSignal.Dispatch();
        }
    }
}
