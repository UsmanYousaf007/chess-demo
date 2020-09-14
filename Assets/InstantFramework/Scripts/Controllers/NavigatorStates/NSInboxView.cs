namespace TurboLabz.InstantFramework
{
    public class NSInboxView : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.INBOX_VIEW);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.TOPICS_VIEW, NavigatorViewId.LOBBY,
                NavigatorViewId.FRIENDS, NavigatorViewId.SHOP, NavigatorViewId.INVENTORY, NavigatorViewId.ARENA_VIEW);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
                if (viewId == NavigatorViewId.FRIENDS)
                {
                    return new NSFriends();
                }
                if (viewId == NavigatorViewId.SHOP)
                {
                    return new NSShop();
                }
                if (viewId == NavigatorViewId.INVENTORY)
                {
                    return new NSInventory();
                }
                if (viewId == NavigatorViewId.ARENA_VIEW)
                {
                    cmd.loadArenaSignal.Dispatch();
                    return null;
                }
                if (viewId == NavigatorViewId.TOPICS_VIEW)
                {
                    return new NSLessonTopics();
                }
            }
            else if (evt == NavigatorEvent.SHOW_REWARD_DLG)
            {
                return new NSRewardDlgView();
            }
            else if (evt == NavigatorEvent.SHOW_SETTINGS)
            {
                return new NSSettings();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP)
            {
                return new NSShop();
            }
            else if (evt == NavigatorEvent.SHOW_ARENA)
            {
                return new NSArenaView();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }

            return null;
        }
    }
}
