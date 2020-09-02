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
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY,
                NavigatorViewId.FRIENDS, NavigatorViewId.STORE, NavigatorViewId.INVENTORY, NavigatorViewId.ARENA_VIEW);

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
                if (viewId == NavigatorViewId.STORE)
                {
                    return new NSShop();
                }
                if (viewId == NavigatorViewId.INVENTORY)
                {
                    return new NSInventory();
                }
                if (viewId == NavigatorViewId.ARENA_VIEW)
                {
                    return new NSArenaView();
                }
            }
            else if (evt == NavigatorEvent.SHOW_REWARD_DLG)
            {
                return new NSRewardDlgView();
            }

            return null;
        }
    }
}
