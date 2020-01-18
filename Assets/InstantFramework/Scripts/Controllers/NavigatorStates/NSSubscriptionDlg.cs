namespace TurboLabz.InstantFramework
{
    public class NSSubscriptionDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.SUBSCRIPTION_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY, NavigatorViewId.THEME_SELECTION, NavigatorViewId.MULTIPLAYER, NavigatorViewId.CPU, NavigatorViewId.SETTINGS);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
                else if (viewId == NavigatorViewId.THEME_SELECTION)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.SUBSCRIPTION_DLG);
                    return new NSThemeSelectionDlg();
                }
                else if (viewId == NavigatorViewId.SETTINGS)
                {
                    return new NSSettings();
                }
                else if (viewId == NavigatorViewId.MULTIPLAYER)
                {
                    return new NSMultiplayer();
                }
                if (viewId == NavigatorViewId.CPU)
                {
                    return new NSCPU();
                }
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_CONFIRM_DLG)
            {
                return new NSConfirmDlg();
            }
            else if (evt == NavigatorEvent.SHOW_THEME_SELECTION_DLG)
            {
                return new NSThemeSelectionDlg();
            }

            return null;
        }
    }
}
