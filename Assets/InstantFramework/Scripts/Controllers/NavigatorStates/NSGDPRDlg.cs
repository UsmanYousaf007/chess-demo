namespace TurboLabz.InstantFramework
{
    public class NSGDPRDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.GDPR_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY);

            if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }

            return null;
        }
    }
}
