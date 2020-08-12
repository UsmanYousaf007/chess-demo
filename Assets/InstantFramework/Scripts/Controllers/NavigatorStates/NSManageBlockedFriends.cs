namespace TurboLabz.InstantFramework
{
    public class NSManageBlockedFriends : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.MANAGE_BLOCKED_FRIENDS);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.FRIENDS);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.FRIENDS)
                {
                    return new NSFriends();
                }
            }
            else if (evt == NavigatorEvent.SHOW_FRIENDS)
            {
                return new NSFriends();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_THEME_SELECTION_DLG)
            {
                return new NSThemeSelectionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SETTINGS)
            {
                return new NSSettings();
            }
            else if (evt == NavigatorEvent.SHOW_EARN_REWARDS_DLG)
            {
                return new NSEarnRewardsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_CONFIRM_DLG)
            {
                return new NSConfirmDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP)
            {
                return new NSShop();
            }

            return null;
        }
    }
}
