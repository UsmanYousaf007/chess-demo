namespace TurboLabz.InstantFramework
{
    public class NSBuyGameAanalysisView : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.BUY_GAME_ANALYSIS_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.MULTIPLAYER);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.MULTIPLAYER)
                {
                    return new NSLobby();
                }
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            return null;
        }
    }
}
