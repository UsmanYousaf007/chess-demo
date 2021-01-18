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
                    cmd.loadLobbySignal.Dispatch();
                    return null;
                }
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }

            return null;
        }
    }
}
