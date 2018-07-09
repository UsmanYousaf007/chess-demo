/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-21 15:56:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class CCSAnnounceResults : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            IChessboardModel model = cmd.chessboardModel;
            bool playerWins = (model.winnerId == cmd.playerModel.id) ? true : false;

            cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG);

            ResultsVO vo = new ResultsVO();
            vo.reason = model.gameEndReason;
            vo.playerWins = playerWins;
            vo.currentEloScore = cmd.playerModel.eloScore;
            vo.eloScoreDelta = vo.currentEloScore - cmd.matchInfoModel.playerPrematchElo;

            cmd.updateResultsDialogSignal.Dispatch(vo);
        }
    }
}
