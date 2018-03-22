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

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public class UpdateAppMediator : Mediator
    {
        // View injection
        [Inject] public UpdateAppView view { get; set; }

        [Inject] public UpdateAppSignal updateAppSignal { get; set; }

        public override void OnRegister()
        {
            view.updateButtonClickedSignal.AddListener(OnUpdateButtonClicked);
            view.Init();
        }

        public override void OnRemove()
        {
            view.updateButtonClickedSignal.RemoveListener(OnUpdateButtonClicked);
        }

        [ListensTo(typeof(ShowViewSignal))]
        public void OnShowView(ViewId viewId)
        {
            if (viewId == ViewId.UPDATE_APP) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(HideViewSignal))]
        public void OnHideView(ViewId viewId)
        {
            if (viewId == ViewId.UPDATE_APP)
            {
                view.Hide();
            }
        }

        private void OnUpdateButtonClicked()
        {
            updateAppSignal.Dispatch();
        }
    }
}
