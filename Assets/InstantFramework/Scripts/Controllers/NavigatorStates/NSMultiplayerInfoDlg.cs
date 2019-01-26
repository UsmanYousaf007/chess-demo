namespace TurboLabz.InstantFramework
{
    public class NSMultiplayerInfoDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.MULTIPLAYER_INFO_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_MULTIPLAYER ||
                evt == NavigatorEvent.ESCAPE)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new NSMultiplayerResultsDlg();
            }

            return null;
        }
    }
}

