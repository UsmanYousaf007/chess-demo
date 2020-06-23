/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class NSMultiplayer : NS
    {
        long timeAtScreenShown;

        public override void RenderDisplayOnEnter()
        {
            timeAtScreenShown = cmd.backendService.serverClock.currentTimestamp;
            ShowView(NavigatorViewId.MULTIPLAYER);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            var matchInfo = cmd.matchInfoModel.activeMatch == null ? cmd.matchInfoModel.lastCompletedMatch : cmd.matchInfoModel.activeMatch;
            var timeSpent = (cmd.backendService.serverClock.currentTimestamp - timeAtScreenShown) / 1000;

            if (matchInfo != null)
            {
                if (matchInfo.isLongPlay)
                {
                    cmd.preferencesModel.timeSpentLongMatch += timeSpent;
                }
                else if (matchInfo.isTenMinGame)
                {
                    cmd.preferencesModel.timeSpent10mMatch += timeSpent;
                }
                else if (matchInfo.isOneMinGame)
                {
                    cmd.preferencesModel.timeSpent1mMatch += timeSpent;
                }
                else
                {
                    cmd.preferencesModel.timeSpent5mMatch += timeSpent;
                }
            }

            if (evt == NavigatorEvent.SHOW_MULTIPLAYER_EXIT_DLG)
            {
                TLUtils.LogUtil.LogNullValidation(cmd.matchInfoModel.activeChallengeId, "cmd.matchInfoModel.activeChallengeId");

                // TODO: This is a workaround HACK. Ideally the active challenge id will not become null while still on the board!
                // ContainsKey will crash if null passed as parameter
                if (cmd.matchInfoModel.activeChallengeId == null)
                {
                    return new NSMultiplayerResultsDlg();
                }

                // TODO: This is a workaround HACK. Investigate why the chessboard disappeared from the list for this challenge id?!
                bool p = cmd.multiplayerChessboardModel.chessboards.ContainsKey(cmd.matchInfoModel.activeChallengeId);
                if (p == false)
                {
                    return new NSMultiplayerResultsDlg();
                }

                if (p == true && cmd.multiplayerChessboardModel.chessboards[cmd.matchInfoModel.activeChallengeId].inPlaybackMode)
                {
                    return new NSMultiplayerResultsDlg();
                }
                else
                {
                    return new NSMultiplayerExitDlg();
                }
            }
            else if (evt == NavigatorEvent.ESCAPE)
            {
                if (cmd.matchInfoModel.activeChallengeId == null)
                {
                    return new NSMultiplayerResultsDlg();
                }

                bool p = cmd.multiplayerChessboardModel.chessboards.ContainsKey(cmd.matchInfoModel.activeChallengeId);
                if (p == true && cmd.multiplayerChessboardModel.chessboards[cmd.matchInfoModel.activeChallengeId].inPlaybackMode)
                {
                    return new NSMultiplayerResultsDlg();
                }
                else
                {
                    if (cmd.matchInfoModel.activeMatch != null && cmd.matchInfoModel.activeMatch.isLongPlay)
                    {
                        cmd.exitLongMatchSignal.Dispatch();
                        cmd.cancelHintSingal.Dispatch();
                        return null;
                    }
                    else if (cmd.matchInfoModel.lastCompletedMatch != null && cmd.matchInfoModel.lastCompletedMatch.isLongPlay)
                    {
                        return new NSMultiplayerResultsDlg();
                    }
                    else
                    {
                        return new NSMultiplayerExitDlg();
                    }
                }
            }
            else if (evt == NavigatorEvent.SHOW_SHARE_SCREEN_DLG)
            {
                return new NSShareDlg();
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG)
            {
                return new NSMultiplayerResultsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_PROMO_DLG)
            {
                return new NSMultiplayerPromo();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_FIND_DLG)
            {
                return new NSMultiplayerFindDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_ACCEPT_DLG)
            {
                return new NSMultiplayerAcceptDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_SAFE_MOVE_DLG)
            {
                return new NSMultiplayerSafeMoveDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_FIFTY_MOVE_DRAW_DLG)
            {
                return new NSMultiplayerFiftyMoveDrawDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_THREEFOLD_REPEAT_DRAW_DLG)
            {
                return new NSMultiplayerThreeFoldRepeatDrawDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_CHAT_DLG)
            {
                return new NSMultiplayerChatDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_INFO_DLG)
            {
                return new NSMultiplayerInfoDlg();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_PROFILE_DLG)
            {
                return new NSProfileDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
            }

            return null;
        }
    }
}

