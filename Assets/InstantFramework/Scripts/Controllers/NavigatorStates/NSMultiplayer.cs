/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework
{
    public class NSMultiplayer : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.MULTIPLAYER);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_MULTIPLAYER_EXIT_DLG)
            {
                // TODO: This is a workaround HACK. Investigate why the chessboard disappeared from the list for this challenge id?!
                bool p = cmd.multiplayerChessboardModel.chessboards.ContainsKey(cmd.matchInfoModel.activeChallengeId);
                if (p == false && cmd.matchInfoModel.activeChallengeId != null)
                {
                    return new NSMultiplayerResultsDlg();
                }

                if (cmd.matchInfoModel.activeChallengeId == null || cmd.multiplayerChessboardModel.chessboards[cmd.matchInfoModel.activeChallengeId].inPlaybackMode)
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
                if (cmd.matchInfoModel.activeChallengeId == null || cmd.multiplayerChessboardModel.chessboards[cmd.matchInfoModel.activeChallengeId].inPlaybackMode)
                {
                    return new NSMultiplayerResultsDlg();
                }
                else
                {
                    if (cmd.matchInfoModel.activeMatch.isLongPlay)
                    {
                        cmd.exitLongMatchSignal.Dispatch();
                        return null;
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
            else if (evt == NavigatorEvent.SHOW_SPOT_PURCHASE)
            {
                return new NSSpotPurchaseDlg();
            }

            return null;
        }
    }
}

