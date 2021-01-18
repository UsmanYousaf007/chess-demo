
namespace TurboLabz.InstantFramework
{
    public class NSRewardDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.REWARD_DLG_V2);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE)
            {
                NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY, NavigatorViewId.RATE_APP_DLG, NavigatorViewId.CHAMPIONSHIP_RESULT_DLG, NavigatorViewId.LEAGUE_PROMOTION_DLG, NavigatorViewId.REWARD_DLG, NavigatorViewId.SPOT_COIN_PURCHASE_DLG, NavigatorViewId.CHAMPIONSHIP_NEW_RANK_DLG, NavigatorViewId.DAILY_REWARD_DLG, NavigatorViewId.SELECT_TIME_MODE);

                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
                else if (viewId == NavigatorViewId.RATE_APP_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.REWARD_DLG_V2);
                    return new NSRateAppDlg();
                }
                else if (viewId == NavigatorViewId.CHAMPIONSHIP_RESULT_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.REWARD_DLG_V2);
                    return new NSChampionshipResultDlg();
                }
                else if (viewId == NavigatorViewId.LEAGUE_PROMOTION_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.REWARD_DLG_V2);
                    return new NSLeaguePromotionDlg();
                }
                else if (viewId == NavigatorViewId.REWARD_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.REWARD_DLG_V2);
                    return new NSRewardDlgView();
                }
                else if (viewId == NavigatorViewId.SPOT_COIN_PURCHASE_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.REWARD_DLG_V2);
                    return new NSSpotCoinPurchaseDlg();
                }
                else if (viewId == NavigatorViewId.CHAMPIONSHIP_NEW_RANK_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.REWARD_DLG_V2);
                    return new NSChampionshipNewRankDlg();
                }
                else if (viewId == NavigatorViewId.DAILY_REWARD_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.REWARD_DLG_V2);
                    return new NSDailyRewardDlg();
                }
                else if (viewId == NavigatorViewId.SELECT_TIME_MODE)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.REWARD_DLG_V2);
                    return new NSSelectTimeMode();
                }
            }

            return null;
        }
    }
}
