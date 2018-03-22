/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-11 13:42:50 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public class MatchmakingMediator : Mediator
    {
        // View injection
        [Inject] public MatchmakingView view { get; set; }

        // Dispatch signals
        [Inject] public GetGameStartTimeSignal getGameStartTimeSignal { get; set; }

        public override void OnRegister()
        {
            view.viewDurationCompleteSignal.AddListener(OnViewDurationComplete);
            view.Init();
        }

        public override void OnRemove()
        {
            view.viewDurationCompleteSignal.RemoveListener(OnViewDurationComplete);
        }

        [ListensTo(typeof(UpdateMatchmakingViewPreMatchFoundSignal))]
        public void OnUpdateViewPreMatchFound(PreMatchmakingVO vo)
        {
            view.UpdateViewPreMatchFound(vo);
        }

        [ListensTo(typeof(UpdateMatchmakingViewPostMatchFoundSignal))]
        public void OnUpdateViewPostMatchFound(PostMatchmakingVO vo)
        {
            view.UpdateViewPostMatchFound(vo);
        }

        // We don't need to do the same for the opponent profile picture because
        // the matchmaking command ensures that we only update the view after we
        // get the opponent's profile picture if there is any.
        [ListensTo(typeof(UpdatePlayerProfilePictureSignal))]
        public void OnUpdatePlayerProfilePicture(Sprite sprite)
        {
            view.UpdatePlayerProfilePicture(sprite);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MATCH_MAKING) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MATCH_MAKING)
            {
                view.Hide();
            }
        }

        private void OnViewDurationComplete()
        {
            getGameStartTimeSignal.Dispatch();
        }
    }
}
