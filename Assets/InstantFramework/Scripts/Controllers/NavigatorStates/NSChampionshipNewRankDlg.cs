
namespace TurboLabz.InstantFramework
{
    public class NSChampionshipNewRankDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.CHAMPIONSHIP_NEW_RANK_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_REWARD_DLG_V2)
            {
                return new NSRewardDlg();
            }

            return null;
        }
    }
}
