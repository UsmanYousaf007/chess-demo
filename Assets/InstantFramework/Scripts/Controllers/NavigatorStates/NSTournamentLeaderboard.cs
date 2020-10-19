/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class NSTournamentLeaderboard : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW);
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
                }
                else if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
            }
            else if (evt == NavigatorEvent.SHOW_PROFILE_DLG)
            {
                return new NSProfileDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_SETTINGS)
            {
                return new NSSettings();
            }
            else if (evt == NavigatorEvent.SHOW_START_GAME_DLG)
            {
                return new NSStartGameDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP)
            {
                return new NSShop();
            }
            else if (evt == NavigatorEvent.SHOW_INVENTORY)
            {
                return new NSInventory();
            }
            else if (evt == NavigatorEvent.SHOW_INBOX)
            {
                return new NSInboxView();
            }
            else if (evt == NavigatorEvent.SHOW_CHEST_INFO_DLG)
            {
                return new NSChestInfoDlgView();
            }
            else if (evt == NavigatorEvent.SHOW_LEAGUE_PERKS_VIEW)
            {
                return new NSLeaguePerksView();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_PURCHASE)
            {
                return new NSSpotPurchase();
            }
            else if (evt == NavigatorEvent.SHOW_ARENA)
            {
                return new NSArenaView();
            }
            else if (evt == NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG)
            {
                return new NSTournamentOverDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_INVENTORY)
            {
                return new NSSpotInventory();
            }
            else if (evt == NavigatorEvent.SHOW_REWARD_DLG)
            {
                return new NSRewardDlgView();
            }

            return null;
        }
    }
}

