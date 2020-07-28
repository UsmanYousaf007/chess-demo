namespace TurboLabz.InstantFramework
{
    public class NSOpponentStatusDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.OPPONENT_STATUS_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY, NavigatorViewId.CPU, NavigatorViewId.MULTIPLAYER, NavigatorViewId.SUBSCRIPTION_DLG, NavigatorViewId.MANAGE_BLOCKED_FRIENDS, NavigatorViewId.TOPICS_VIEW);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
                else if (viewId == NavigatorViewId.SUBSCRIPTION_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.OPPONENT_STATUS_DLG);
                    return new NSSubscriptionDlg();
                }
                else if (viewId == NavigatorViewId.CPU)
                {
                    return new NSCPU();
                }
                else if (viewId == NavigatorViewId.MULTIPLAYER)
                {
                    return new NSMultiplayer();
                }
                else if (viewId == NavigatorViewId.MANAGE_BLOCKED_FRIENDS)
                {
                    return new NSManageBlockedFriends();
                }
                else if (viewId == NavigatorViewId.TOPICS_VIEW)
                {
                    return new NSLessonTopics();
                }
            }
            return null;
        }
    }
}
