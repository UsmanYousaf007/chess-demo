/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.InstantFramework;


namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        [Inject] public ExitLongMatchSignal exitLongMatchSignal { get; set; }

        public void OnRegisterBotBar()
        {
            view.InitBotBar();
            view.backToFriendsButton.onClick.AddListener(OnBackToFriends);
        }

        public void OnBackToFriends()
        {
            exitLongMatchSignal.Dispatch();
        }
    }
}
