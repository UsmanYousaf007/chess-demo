
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
                NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY, NavigatorViewId.RATE_APP_DLG, NavigatorViewId.CHAMPIONSHIP_RESULT_DLG, NavigatorViewId.LEAGUE_PROMOTION_DLG, NavigatorViewId.REWARD_DLG);

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
            }

            return null;
        }
    }
}
