namespace TurboLabz.InstantFramework
{
    public class NSEarnRewardsDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.EARN_REWARDS_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY, NavigatorViewId.FRIENDS, NavigatorViewId.MANAGE_BLOCKED_FRIENDS, NavigatorViewId.TOPICS_VIEW);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
                else if (viewId == NavigatorViewId.FRIENDS)
                {
                    return new NSFriends();
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

