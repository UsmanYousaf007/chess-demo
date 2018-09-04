/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        // Dispatch Signals
        [Inject] public DeclineSignal declineSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        public void OnRegisterAccept()
        {
            view.InitAccept();

            view.acceptButtonClickedSignal.AddListener(OnAcceptClicked);
            view.declineButtonClickedSignal.AddListener(OnDeclineClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowAcceptView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_ACCEPT_DLG) 
            {
                view.ShowAccept();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideAcceptView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_ACCEPT_DLG)
            {
                view.HideAccept();
            }
        }

        private void OnDeclineClicked()
        {
         //   declineSignal.Dispatch();
        
        }

        private void OnAcceptClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
        }
    }
}
