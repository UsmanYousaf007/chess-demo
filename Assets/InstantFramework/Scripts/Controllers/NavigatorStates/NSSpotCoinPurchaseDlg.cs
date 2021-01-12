namespace TurboLabz.InstantFramework
{
    public class NSSpotCoinPurchaseDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.SPOT_COIN_PURCHASE_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE)
            {
                NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY);

                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_PURCHASE)
            {
                return new NSSpotPurchase();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_REMOVE_ADS_DLG)
            {
                return new NSPromotionRemoveAdsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_REMOVE_ADS_SALE_DLG)
            {
                return new NSPromotionRemoveAdsSaleDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_CHESS_COURSE_DLG)
            {
                return new NSPromotionChessCourseDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_CHESS_SETS_BUNDLE_DLG)
            {
                return new NSPromotionChessSetsBundleDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_ELITE_BUNDLE_DLG)
            {
                return new NSPromotionEliteBundleDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_WELCOME_BUNDLE_DLG)
            {
                return new NSPromotionWelcomeBundleDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_SALE_DLG)
            {
                return new NSSubscriptionSaleDlg();
            }
            else if (evt == NavigatorEvent.SHOW_REWARD_DLG)
            {
                return new NSRewardDlgView();
            }
            else if (evt == NavigatorEvent.SHOW_LOGIN_DLG)
            {
                return new NSLoginDlg();
            }
            else if (evt == NavigatorEvent.SHOW_REWARD_DLG_V2)
            {
                return new NSRewardDlg();
            }

            return null;
        }
    }
}
