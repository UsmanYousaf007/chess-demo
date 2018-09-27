/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.InstantFramework;


namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        public void OnRegisterBotBar()
        {
            view.InitBotBar();
            view.backToFriendsSignal.AddListener(OnBackToFriends);
        }

        public void OnBackToFriends(bool showAd)
        {
            if (showAd)
            {
                showAdSignal.Dispatch(false, AdPlacementIds.AD_PLACEMENT_ENDGAME_VIDEO);
            }
            exitLongMatchSignal.Dispatch();
        }
    }
}
