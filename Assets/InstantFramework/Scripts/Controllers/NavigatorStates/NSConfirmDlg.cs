namespace TurboLabz.InstantFramework
{
    public class NSConfirmDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.CONFIRM_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.STORE, NavigatorViewId.SPOT_PURCHASE_DLG, NavigatorViewId.LOBBY);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.STORE)
                {
                    return new NSStore();
                }
                else if (viewId == NavigatorViewId.SPOT_PURCHASE_DLG)
                {
                    return new NSSpotPurchaseDlg();
                }
                else if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            return null;
        }
    }
}
