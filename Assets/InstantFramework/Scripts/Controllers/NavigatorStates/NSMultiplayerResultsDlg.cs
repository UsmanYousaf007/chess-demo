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
using TurboLabz.TLUtils;
using System;

namespace TurboLabz.InstantFramework
{
    public class NSMultiplayerResultsDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.MULTIPLAYER_RESULTS_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE)
            {
                try
                {
                    if (cmd.matchInfoModel.lastCompletedMatch.gameEndReason.Equals(Chess.GameEndReason.DECLINED.ToString()))
                    {
                        cmd.exitLongMatchSignal.Dispatch();
                        cmd.cancelHintSingal.Dispatch();
                        return null;
                    }
                    else
                    {
                        cmd.showViewBoardResultsPanelSignal.Dispatch(true);
                        return new NSMultiplayer();
                    }
                }
                catch (Exception e)
                {
                    LogUtil.Log("Last completed match is null " + e, "red");
                }
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_STATS)
            {
                return new NSStats();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_PURCHASE)
            {
                return new NSSpotPurchase();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_INVENTORY)
            {
                return new NSSpotInventory();
            }
            else if (evt == NavigatorEvent.SHOW_CHAMPIONSHIP_NEW_RANK_DLG)
            {
                return new NSChampionshipNewRankDlg();
            }
            else if (evt == NavigatorEvent.SHOW_GAME_BUY_ANALYSIS_DLG)
            {
                return new NSBuyGameAanalysisView();
            }
            else if (evt == NavigatorEvent.SHOW_GAME_ANALYZING_DLG)
            {
                return new NSGameAnalyzingDlg();
            }
            else if (evt == NavigatorEvent.SHOW_FRIENDS)
            {
                return new NSFriends();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_GAME_ANALYSIS)
            {
                return new NSMultiplayerGameAnalysis();
            }

            return null;
        }
    }
}

