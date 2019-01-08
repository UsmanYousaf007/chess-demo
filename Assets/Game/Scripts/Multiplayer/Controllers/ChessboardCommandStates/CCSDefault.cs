/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-16 14:30:36 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public class CCSDefault : CCS
    {
        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            Chessboard chessboard = cmd.activeChessboard;
            IChessService chessService = cmd.chessService;
            IChessAiService chessAiService = cmd.chessAiService;

            if (cmd.chessboardEvent == ChessboardEvent.GAME_STARTED)
            {
                if (chessboard.overrideFen != null)
                {
                    chessService.NewGame(chessboard.fen, chessboard.squares);
                }
                else
                {
                    chessService.NewGame(chessboard.squares);
                    ProcessResume(cmd);
                }

                // Initialize the Ai service
                if (chessboard.isAiGame)
                {
                    if (chessboard.overrideAiResignBehaviour == AiOverrideResignBehaviour.ALWAYS)
                    {
                        chessboard.aiWillResign = true;    
                    }
                    else if (chessboard.overrideAiResignBehaviour == AiOverrideResignBehaviour.NEVER)
                    {
                        chessboard.aiWillResign = false;
                    }
                    else 
                    {
                        chessboard.aiWillResign = (UnityEngine.Random.Range(0, 100) < BotSettings.AI_RESIGN_CHANCE);
                    }

                    chessAiService.NewGame();
                }

                // Wait for player turn or execute Ai turn
                if (chessboard.isPlayerTurn)
                {
                    if (chessboard.threefoldRepeatDrawAvailable)
                    {
                        return new CCSThreefoldRepeatDrawOnOpponentTurnAvailable();
                    }
                    else if (chessboard.fiftyMoveDrawAvailable)
                    {
                        return new CCSFiftyMoveDrawOnOpponentTurnAvailable();
                    }

                    return new CCSPlayerTurn();
                }
                else
                {
                    if (chessboard.isAiGame)
                    {
                        cmd.aiTurnSignal.Dispatch();
                    }

                    return new CCSOpponentTurn();
                }
            }

            return null;
        }


    }
}
