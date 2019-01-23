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
            if (evt == NavigatorEvent.SHOW_CPU ||
                evt == NavigatorEvent.ESCAPE)
            {
                return new NSMultiplayer();
            }

            return null;
        }
    }
}

