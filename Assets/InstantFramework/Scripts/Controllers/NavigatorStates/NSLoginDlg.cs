namespace TurboLabz.InstantFramework
{
    public class NSLoginDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.LOGIN_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE)
            {
                NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY, NavigatorViewId.RATE_APP_DLG, NavigatorViewId.SPOT_COIN_PURCHASE_DLG);

                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
                else if (viewId == NavigatorViewId.RATE_APP_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.LOGIN_DLG);
                    return new NSRateAppDlg();
                }
                else if (viewId == NavigatorViewId.SPOT_COIN_PURCHASE_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.LOGIN_DLG);
                    return new NSSpotCoinPurchaseDlg();
                }

                return null;
            }

            return null;
        }
    }
}
