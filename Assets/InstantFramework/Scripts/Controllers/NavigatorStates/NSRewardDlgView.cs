namespace TurboLabz.InstantFramework
{
    public class NSRewardDlgView : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.REWARD_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW, NavigatorViewId.INBOX_VIEW, NavigatorViewId.LOBBY, NavigatorViewId.RATE_APP_DLG);

            if (evt == NavigatorEvent.ESCAPE)
            {
                cmd.hideViewSignal.Dispatch(NavigatorViewId.REWARD_DLG);
                if (viewId == NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW)
                {
                    return new NSTournamentLeaderboard();
                }
                else if (viewId == NavigatorViewId.INBOX_VIEW)
                {
                    return new NSInboxView();
                }
                else if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
                else if (viewId == NavigatorViewId.RATE_APP_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.REWARD_DLG);
                    return new NSRateAppDlg();
                }
            }
            else if (evt == NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS)
            {
                return new NSTournamentLeaderboard();
            }
            else if (evt == NavigatorEvent.SHOW_INBOX)
            {
                return new NSInboxView();
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_REWARD_DLG_V2)
            {
                return new NSRewardDlg();
            }

            return null;
        }
    }
}
