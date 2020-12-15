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
                cmd.loginAsGuestSignal.Dispatch();
                return null;
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
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
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
            }

            return null;
        }
    }
}
