﻿namespace TurboLabz.InstantFramework
{
    public class NSShop : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.SHOP);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.ESCAPE)
            {
                cmd.androidNativeService.SendToBackground();
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_FRIENDS)
            {
                return new NSFriends();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_CONFIRM_DLG)
            {
                return new NSConfirmDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SETTINGS)
            {
                return new NSSettings();
            }

            return null;
        }
    }
}
