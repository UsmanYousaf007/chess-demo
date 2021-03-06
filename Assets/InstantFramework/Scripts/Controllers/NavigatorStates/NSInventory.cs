namespace TurboLabz.InstantFramework
{
    public class NSInventory : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.INVENTORY);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.ESCAPE)
            {
                cmd.androidNativeService.SendToBackground();
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_FRIENDS)
            {
                return new NSFriends();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP)
            {
                return new NSShop();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_CONFIRM_DLG)
            {
                return new NSConfirmDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SETTINGS)
            {
                return new NSSettings();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_PURCHASE)
            {
                return new NSSpotPurchase();
            }
            else if (evt == NavigatorEvent.SHOW_ARENA)
            {
                return new NSArenaView();
            }
            else if (evt == NavigatorEvent.SHOW_INBOX)
            {
                return new NSInboxView();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_INVENTORY)
            {
                return new NSSpotInventory();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_SALE_DLG)
            {
                return new NSSubscriptionSaleDlg();
            }

            return null;
        }
    }
}
