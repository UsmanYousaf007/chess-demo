namespace TurboLabz.InstantFramework
{
    public class NSInboxView : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.INBOX_VIEW);
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
