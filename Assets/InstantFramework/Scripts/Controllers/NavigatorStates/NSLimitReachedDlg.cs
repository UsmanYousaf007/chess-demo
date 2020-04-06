namespace TurboLabz.InstantFramework
{
    public class NSLimitReachedDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.CREATE_MATCH_LIMIT_REACHED_DIALOG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY, NavigatorViewId.FRIENDS);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
                else if (viewId == NavigatorViewId.FRIENDS)
                {
                    return new NSFriends();
                }
            }

            return null;
        }
    }
}