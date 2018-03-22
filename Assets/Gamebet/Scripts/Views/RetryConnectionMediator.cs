/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 18:53:20 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public class RetryConnectionMediator : Mediator
    {
        // Dispatch signals
        [Inject] public ConnectBackendSignal connectBackendSignal { get; set; }

        // View injection
        [Inject] public RetryConnectionView view { get; set; }

        public override void OnRegister()
        {
            view.retryConnectionButtonClickedSignal.AddListener(OnRetryConnectionButtonClicked);
            view.Init();
        }

        public override void OnRemove()
        {
            view.retryConnectionButtonClickedSignal.RemoveListener(OnRetryConnectionButtonClicked);
        }

        [ListensTo(typeof(UpdateRetryConnectionMessageSignal))]
        public void OnUpdateRetryConnectionMessage(BackendResult message)
        {
            view.SetErrorMessage(message);
        }

        [ListensTo(typeof(ShowViewSignal))]
        public void OnShowView(ViewId viewId)
        {
            if (viewId == ViewId.RETRY_CONNECTION) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(HideViewSignal))]
        public void OnHideView(ViewId viewId)
        {
            if (viewId == ViewId.RETRY_CONNECTION)
            {
                view.Hide();
            }
        }

        private void OnRetryConnectionButtonClicked()
        {
            connectBackendSignal.Dispatch();
        }
    }
}
