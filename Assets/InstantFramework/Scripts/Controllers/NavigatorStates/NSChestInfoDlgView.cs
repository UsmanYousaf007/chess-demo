namespace TurboLabz.InstantFramework
{
    public class NSChestInfoDlgView : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.CHEST_INFO_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW, NavigatorViewId.ARENA_VIEW);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW)
                {
                    return new NSTournamentLeaderboard();
                }
                else if(viewId == NavigatorViewId.ARENA_VIEW)
                {
                    return new NSArenaView();
                }
            }

            return null;
        }
    }
}
