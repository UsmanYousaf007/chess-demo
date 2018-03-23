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
using TurboLabz.InstantFramework;
using System.Collections.Generic;
using TurboLabz.Chess;
using System;
using TurboLabz.TLUtils;
using UnityEngine;


namespace TurboLabz.InstantChess
{
    public class LoadGameCommand : Command
    {
        // Models
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        // Services
        [Inject] public ILocalDataService localDataService { get; set; }

        public override void Execute()
        {
            if (!localDataService.FileExists(SaveKeys.CPU_SAVE_FILENAME))
            {
                LogUtil.Log("No saved game found.", "yellow");
                return;
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(SaveKeys.CPU_SAVE_FILENAME);

                // CPU MENU MODEL
                cpuGameModel.cpuStrength = reader.Read<int>(SaveKeys.CPU_STRENGTH);
                cpuGameModel.durationIndex = reader.Read<int>(SaveKeys.DURATION_INDEX);
                cpuGameModel.playerColorIndex = reader.Read<int>(SaveKeys.PLAYER_COLOR_INDEX);
                cpuGameModel.inProgress = reader.Read<bool>(SaveKeys.IN_PROGRESS);

                if (!cpuGameModel.inProgress)
                {
                    reader.Close();
                    return;
                }
                    
                if (Debug.isDebugBuild)
                {
                    cpuGameModel.devFen = reader.Read<string>(SaveKeys.DEV_FEN);
                }

                // CHESSBOARD MODEL
                chessboardModel.gameDuration = TimeSpan.FromTicks(reader.Read<long>(SaveKeys.GAME_DURATION));
                chessboardModel.playerTimer = TimeSpan.FromTicks(reader.Read<long>(SaveKeys.PLAYER_TIMER));
                chessboardModel.opponentTimer = TimeSpan.FromTicks(reader.Read<long>(SaveKeys.OPPONENT_TIMER));
                chessboardModel.playerColor = reader.Read<ChessColor>(SaveKeys.PLAYER_COLOR);
                chessboardModel.opponentColor = reader.Read<ChessColor>(SaveKeys.OPPONENT_COLOR);
                chessboardModel.availableHints = reader.Read<int>(SaveKeys.AVAILABLE_HINTS);

                List<string> moveList = reader.ReadList<string>(SaveKeys.MOVE_LIST);
                chessboardModel.moveList = new List<ChessMove>();

                foreach (string json in moveList)
                {
                    ChessMove move = JsonUtility.FromJson<ChessMove>(json);

                    // Unity Json Utility does not respect nulls and converts
                    // them to empty strings. So we undo that here.
                    // TODO: replace unity built in lib with proper json library
                    if (move.promo == "")
                    {
                        move.promo = null;
                    }

                    chessboardModel.moveList.Add(move);
                }

                reader.Close();

                /*
                LogUtil.Log("Loaded game", "yellow");

                // CPU MENU MODEL
                LogUtil.Log("Loaded.CPU_STRENGTH: " + cpuGameModel.cpuStrength, "yellow");
                LogUtil.Log("Loaded.DURATION_INDEX: " + cpuGameModel.durationIndex, "yellow");
                LogUtil.Log("Loaded.IN_PROGRESS: " + cpuGameModel.inProgress, "yellow");
                LogUtil.Log("Loaded.PLAYER_COLOR_INDEX: " + cpuGameModel.playerColorIndex, "yellow");
                LogUtil.Log("Loaded.DEV_FEN: " + cpuGameModel.devFen, "yellow");

                // CHESSBOARD MODEL
                LogUtil.Log("Loaded.PLAYER_TIMER: " +  chessboardModel.playerTimer.TotalMilliseconds, "yellow");
                LogUtil.Log("Loaded.OPPONENT_TIMER: " +  chessboardModel.opponentTimer.TotalMilliseconds, "yellow");
                LogUtil.Log("Loaded.PLAYER_COLOR: " +  chessboardModel.playerColor, "yellow");
                LogUtil.Log("Loaded.OPPONENT_COLOR: " +  chessboardModel.opponentColor, "yellow");

                foreach(string move in moveList)
                {
                    LogUtil.Log("Move: " + move, "yellow");
                }
                */
            }
            catch (Exception e)
            {
                LogUtil.Log("Corrupt saved game! " + e, "red");
                chessboardModel.Reset();
                cpuGameModel.Reset();
                cpuGameModel.inProgress = false;
                localDataService.DeleteFile(SaveKeys.CPU_SAVE_FILENAME);

            }
        }
    }
}
