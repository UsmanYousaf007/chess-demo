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
            NavigatorViewId viewId = CameFrom(NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW, NavigatorViewId.INBOX_VIEW);

            if (evt == NavigatorEvent.ESCAPE)
            {
                cmd.hideViewSignal.Dispatch(NavigatorViewId.REWARD_DLG);
                if (viewId == NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW)
                {
                    return new NSTournamentLeaderboard();

                }
                if (viewId == NavigatorViewId.INBOX_VIEW)
                {
                    return new NSInboxView();
                }
            }

            return null;
        }
    }
}
