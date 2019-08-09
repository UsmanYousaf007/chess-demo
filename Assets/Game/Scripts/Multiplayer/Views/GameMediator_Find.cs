/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:03:30 UTC+05:00
/// 
/// @description
/// [add_description_here]


using TurboLabz.InstantFramework;
using UnityEngine;
using TurboLabz.TLUtils;


namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        public void OnRegisterFind()
        {
            view.InitFind();
            view.findMatchTimeoutSignal.AddListener(OnFindMatchTimeout);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowFind(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_FIND_DLG) 
            {
                view.ShowFind();
                view.FindMatchTimeoutEnable(true, 30);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideFind(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_FIND_DLG)
            {
                view.HideFind();
            }
        }

        [ListensTo(typeof(MatchFoundSignal))]
        public void OnMatchFound(ProfileVO vo)
        {
            view.MatchFound(vo);
        }

        public void OnFindMatchTimeout()
        {
            //loadLobbySignal.Dispatch();
            loadHomeSignal.Dispatch();
        }
    }
}
