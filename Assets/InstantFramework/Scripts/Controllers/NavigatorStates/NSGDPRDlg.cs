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

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
               
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }

            return null;
        }
    }
}
