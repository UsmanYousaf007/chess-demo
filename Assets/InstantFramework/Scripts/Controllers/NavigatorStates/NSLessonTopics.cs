namespace TurboLabz.InstantFramework
{
    public class NSLessonTopics : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.TOPICS_VIEW);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.LOBBY)
                {
                    cmd.loadLobbySignal.Dispatch();
                    return null;
                }
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_THEME_SELECTION_DLG)
            {
                return new NSThemeSelectionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SETTINGS)
            {
                return new NSSettings();
            }
            else if (evt == NavigatorEvent.SHOW_EARN_REWARDS_DLG)
            {
                return new NSEarnRewardsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_CONFIRM_DLG)
            {
                return new NSConfirmDlg();
            }
            else if (evt == NavigatorEvent.SHOW_LESSON_VIDEO)
            {
                return new NSLessonVideo();
            }
            else if (evt == NavigatorEvent.SHOW_LESSONS_VIEW)
            {
                return new NSLessonsView();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP)
            {
                return new NSShop();
            }
            else if (evt == NavigatorEvent.SHOW_INBOX)
            {
                return new NSInboxView();
            }
            else if (evt == NavigatorEvent.SHOW_ARENA)
            {
                return new NSArenaView();
            }
            else if (evt == NavigatorEvent.SHOW_INVENTORY)
            {
                return new NSInventory();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_SALE_DLG)
            {
                return new NSSubscriptionSaleDlg();
            }

            return null;
        }
    }
}
