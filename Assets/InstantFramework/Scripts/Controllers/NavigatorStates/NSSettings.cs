/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:37:03 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.InstantFramework
{
    public class NSSettings : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.SETTINGS);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.STATS, NavigatorViewId.INBOX_VIEW, NavigatorViewId.LOBBY, NavigatorViewId.FRIENDS, NavigatorViewId.MANAGE_BLOCKED_FRIENDS, NavigatorViewId.TOPICS_VIEW, NavigatorViewId.SHOP, NavigatorViewId.INVENTORY, NavigatorViewId.ARENA_VIEW);

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
                else if (viewId == NavigatorViewId.MANAGE_BLOCKED_FRIENDS)
                {
                    return new NSManageBlockedFriends();
                }
                else if (viewId == NavigatorViewId.TOPICS_VIEW)
                {
                    cmd.loadTopicsViewSignal.Dispatch();
                    return null;
                }
                else if (viewId == NavigatorViewId.SHOP)
                {
                    return new NSShop();
                }
                else if (viewId == NavigatorViewId.INVENTORY)
                {
                    return new NSInventory();
                }
                else if (viewId == NavigatorViewId.INBOX_VIEW)
                {
                    return new NSInboxView();
                }
                else if (viewId == NavigatorViewId.ARENA_VIEW)
                {
                    cmd.loadArenaSignal.Dispatch();
                    return null;
                }
                else if (viewId == NavigatorViewId.STATS)
                {
                    return new NSStats();
                }
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
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
            else if (evt == NavigatorEvent.SHOW_MANAGE_SUBSCRIPTION)
            {
                return new NSManageSubscription();
            }
            else if (evt == NavigatorEvent.SHOW_TOPICS_VIEW)
            {
                return new NSLessonTopics();
            }
            else if (evt == NavigatorEvent.SHOW_ARENA)
            {
                return new NSArenaView();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
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

