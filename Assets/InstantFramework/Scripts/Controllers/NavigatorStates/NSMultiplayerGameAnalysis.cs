namespace TurboLabz.InstantFramework
{
    public class NSMultiplayerGameAnalysis : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.MULTIPLAYER_GAME_ANALYSIS);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE)
            {
                return new NSMultiplayer();
            }

            return null;
        }
    }
}
