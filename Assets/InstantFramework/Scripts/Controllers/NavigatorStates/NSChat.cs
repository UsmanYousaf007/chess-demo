/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class NSChat : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.CHAT);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.MULTIPLAYER, NavigatorViewId.FRIENDS, NavigatorViewId.LOBBY,
                NavigatorViewId.MANAGE_BLOCKED_FRIENDS, NavigatorViewId.SHOP, NavigatorViewId.INVENTORY,
                NavigatorViewId.TOPICS_VIEW, NavigatorViewId.LESSONS_VIEW, NavigatorViewId.LESSON_VIDEO);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.FRIENDS)
                {
                    return new NSFriends();
                }
                else if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
                else if (viewId == NavigatorViewId.MULTIPLAYER)
                {
                    return new NSMultiplayer();
                }
                else if (viewId == NavigatorViewId.MANAGE_BLOCKED_FRIENDS)
                {
                    return new NSManageBlockedFriends();
                }
                else if (viewId == NavigatorViewId.SHOP)
                {
                    return new NSShop();
                }
                else if (viewId == NavigatorViewId.INVENTORY)
                {
                    return new NSInventory();
                }
                if (viewId == NavigatorViewId.TOPICS_VIEW)
                {
                    cmd.loadTopicsViewSignal.Dispatch();
                    return null;
                }
                else if (viewId == NavigatorViewId.LESSONS_VIEW)
                {
                    cmd.loadLessonsViewSignal.Dispatch();
                    return null;
                }
                else if (viewId == NavigatorViewId.LESSON_VIDEO)
                {
                    return new NSLessonVideo();
                }
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG && viewId == NavigatorViewId.MULTIPLAYER)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new NSMultiplayerResultsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_FIFTY_MOVE_DRAW_DLG && viewId == NavigatorViewId.MULTIPLAYER)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new NSMultiplayerFiftyMoveDrawDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_THREEFOLD_REPEAT_DRAW_DLG && viewId == NavigatorViewId.MULTIPLAYER)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new NSMultiplayerThreeFoldRepeatDrawDlg();
            }

            return null;
        }
    }
}

