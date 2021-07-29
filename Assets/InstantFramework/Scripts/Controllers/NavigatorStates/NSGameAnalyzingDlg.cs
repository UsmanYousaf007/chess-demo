
namespace TurboLabz.InstantFramework
{
    public class NSGameAnalyzingDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.GAME_ANALYZING_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_GAME_ANALYSIS)
            {
                cmd.hideViewSignal.Dispatch(NavigatorViewId.GAME_ANALYZING_DLG);
                return new NSMultiplayerGameAnalysis();
            }

            return null;
        }
    }
}
