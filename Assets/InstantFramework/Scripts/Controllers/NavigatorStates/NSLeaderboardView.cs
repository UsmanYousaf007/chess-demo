namespace TurboLabz.InstantFramework
{
    public class NSLeaderboardView : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.LEADERBOARD_VIEW);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
            }

            return null;
        }
    }
}
