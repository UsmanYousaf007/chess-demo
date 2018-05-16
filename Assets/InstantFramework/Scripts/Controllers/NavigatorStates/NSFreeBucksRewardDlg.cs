/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class NSFreeBucksRewardDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.FREE_BUCKS_REWARD_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_LOBBY||
                evt == NavigatorEvent.ESCAPE)
            {
                return new NSLobby();
            }

            return null;
        }
    }
}

