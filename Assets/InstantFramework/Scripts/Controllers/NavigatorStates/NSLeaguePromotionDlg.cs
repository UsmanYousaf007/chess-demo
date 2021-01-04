
namespace TurboLabz.InstantFramework
{
    public class NSLeaguePromotionDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.LEAGUE_PROMOTION_DLG);
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

            return null;
        }
    }
}
