namespace TurboLabz.InstantFramework
{
    public class NSLeaguePerksView : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.LEAGUE_PERKS_VIEW);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.ARENA_VIEW, NavigatorViewId.LOBBY);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.ARENA_VIEW)
                {
                    cmd.loadArenaSignal.Dispatch();
                    return null;
                }else if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
                //   return null;
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_LESSON_VIDEO)
            {
                return new NSLessonVideo();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_TOPICS_VIEW)
            {
                return new NSLessonTopics();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_PURCHASE)
            {
                return new NSSpotPurchase();
            }
            else if (evt == NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS)
            {
                return new NSTournamentLeaderboard();
            }
            else if (evt == NavigatorEvent.SHOW_INBOX)
            {
                return new NSInboxView();
            }
            else if (evt == NavigatorEvent.SHOW_ARENA)
            {
                return new NSArenaView();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_SALE_DLG)
            {
                return new NSSubscriptionSaleDlg();
            }

            return null;
        }
    }
}
