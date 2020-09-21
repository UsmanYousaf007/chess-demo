/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class NSTournamentOverDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.TOURNAMENT_OVER_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW)
                {
                    //cmd.hideViewSignal.Dispatch(NavigatorViewId.TOURNAMENT_OVER_DLG);
                    //cmd.unlockCurrentJoinedTournamentSignal.Dispatch();
                    //cmd.loadArenaSignal.Dispatch();
                    //return new NSArenaView();

                    return new NSTournamentLeaderboard();
                }
            }
            else if (evt == NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS)
            {
                return new NSTournamentLeaderboard();
            }

            return null;
        }
    }
}
