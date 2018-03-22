/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-11 11:42:52 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Gamebet;
using System.Collections.Generic;
using TurboLabz.Chess;
using TurboLabz.Common;
using System;
using UnityEngine;


namespace TurboLabz.CPUChess
{
    public class SaveGameCommand : Command
    {
        // Models
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        // Services
        [Inject] public ILocalDataService localDataService { get; set; }

        public override void Execute()
        {
            try
            {
                ILocalDataWriter writer = localDataService.OpenWriter(SaveKeys.CPU_SAVE_FILENAME);

                // CPU MENU MODEL
                writer.Write<int>(SaveKeys.CPU_STRENGTH, cpuGameModel.cpuStrength);
                writer.Write<int>(SaveKeys.DURATION_INDEX, cpuGameModel.durationIndex);
                writer.Write<bool>(SaveKeys.IN_PROGRESS, cpuGameModel.inProgress);
                writer.Write<int>(SaveKeys.PLAYER_COLOR_INDEX, cpuGameModel.playerColorIndex);

                if (!cpuGameModel.inProgress)
                {
                    writer.Close();
                    return;
                }
                    
                writer.Write<string>(SaveKeys.DEV_FEN, cpuGameModel.devFen);

                // CHESSBOARD MODEL
                writer.Write<long>(SaveKeys.GAME_DURATION, chessboardModel.gameDuration.Ticks);
                writer.Write<long>(SaveKeys.PLAYER_TIMER, chessboardModel.playerTimer.Ticks);
                writer.Write<long>(SaveKeys.OPPONENT_TIMER, chessboardModel.opponentTimer.Ticks);
                writer.Write<ChessColor>(SaveKeys.PLAYER_COLOR, chessboardModel.playerColor);
                writer.Write<ChessColor>(SaveKeys.OPPONENT_COLOR, chessboardModel.opponentColor);
                writer.Write<int>(SaveKeys.AVAILABLE_HINTS, chessboardModel.availableHints);

                List<string> moveListJson = new List<string>();

                foreach (ChessMove move in chessboardModel.moveList)
                {
                    moveListJson.Add(JsonUtility.ToJson(move));
                }

                writer.WriteList<string>(SaveKeys.MOVE_LIST, moveListJson);

                writer.Close();

                /*
                LogUtil.Log("Saved game", "yellow");

                // CPU MENU MODEL
                LogUtil.Log("SaveKeys.CPU_STRENGTH: " + cpuGameModel.cpuStrength, "yellow");
                LogUtil.Log("SaveKeys.DURATION_INDEX: " + cpuGameModel.durationIndex, "yellow");
                LogUtil.Log("SaveKeys.IN_PROGRESS: " + cpuGameModel.inProgress, "yellow");
                LogUtil.Log("SaveKeys.PLAYER_COLOR_INDEX: " + cpuGameModel.playerColorIndex, "yellow");
                LogUtil.Log("SaveKeys.DEV_FEN: " + cpuGameModel.devFen, "yellow");

                // CHESSBOARD MODEL
                LogUtil.Log("SaveKeys.PLAYER_TIMER: " +  chessboardModel.playerTimer.TotalMilliseconds, "yellow");
                LogUtil.Log("SaveKeys.OPPONENT_TIMER: " +  chessboardModel.opponentTimer.TotalMilliseconds, "yellow");
                LogUtil.Log("SaveKeys.PLAYER_COLOR: " +  chessboardModel.playerColor, "yellow");
                LogUtil.Log("SaveKeys.OPPONENT_COLOR: " +  chessboardModel.opponentColor, "yellow");

                foreach(string move in moveListJson)
                {
                    LogUtil.Log("Move: " + move, "yellow");
                }
                */
            }
            catch (Exception e)
            {
                if (localDataService.FileExists(SaveKeys.CPU_SAVE_FILENAME))
                {
                    localDataService.DeleteFile(SaveKeys.CPU_SAVE_FILENAME);
                }

                LogUtil.Log("Critical error when saving game. File deleted. " + e, "red");
            }
        }
    }
}
