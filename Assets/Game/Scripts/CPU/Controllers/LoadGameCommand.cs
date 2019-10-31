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
using TurboLabz.InstantGame;
using System.Collections.Generic;
using TurboLabz.Chess;
using System;
using TurboLabz.TLUtils;
using UnityEngine;


namespace TurboLabz.CPU
{
	public class LoadGameCommand : Command
	{
		// Models
		[Inject] public ICPUGameModel cpuGameModel { get; set; }
		[Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

		// Services
		[Inject] public ILocalDataService localDataService { get; set; }

		public override void Execute()
		{
			ResetAll();

			if (!localDataService.FileExists(SaveKeys.CPU_SAVE_FILENAME))
			{
				LogUtil.Log("No saved game or settings found.", "yellow");
				return;
			}

			try
			{
				ILocalDataReader reader = localDataService.OpenReader(SaveKeys.CPU_SAVE_FILENAME);

				// CPU MENU MODEL
				cpuGameModel.totalGames = reader.Read<int>(SaveKeys.TOTAL_GAMES);
				cpuGameModel.cpuStrength = reader.Read<int>(SaveKeys.CPU_STRENGTH);
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
                long ticks = reader.Read<long>(SaveKeys.GAME_DURATION);

                // abort if reading an old unsupported time control game
                if (ticks != 0)
                {
                    localDataService.DeleteFile(SaveKeys.CPU_SAVE_FILENAME);
                    ResetAll();
                    return;
                }

				chessboardModel.playerColor = reader.Read<ChessColor>(SaveKeys.PLAYER_COLOR);
				chessboardModel.opponentColor = reader.Read<ChessColor>(SaveKeys.OPPONENT_COLOR);
				chessboardModel.usedHelp = reader.Read<bool>(SaveKeys.USED_HELP);

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


                #region Trimmed Moves for Step System

                chessboardModel.trimmedMoveList = new List<ChessMove>();

                try
                {
                    List<string> trimmedMoveList = reader.ReadList<string>(SaveKeys.TRIMMED_MOVE_LIST);

                    foreach (string json in trimmedMoveList)
                    {
                        ChessMove trimmedMove = JsonUtility.FromJson<ChessMove>(json);

                        // Unity Json Utility does not respect nulls and converts
                        // them to empty strings. So we undo that here.
                        // TODO: replace unity built in lib with proper json library
                        if (trimmedMove.promo == "")
                        {
                            trimmedMove.promo = null;
                        }

                        chessboardModel.trimmedMoveList.Add(trimmedMove);
                    }
                }
                catch (Exception) { }

                #endregion


                reader.Close();
			}
			catch (Exception e)
			{
				LogUtil.Log("Corrupt saved game! " + e, "red");
				localDataService.DeleteFile(SaveKeys.CPU_SAVE_FILENAME);
				ResetAll();
			}
		}

		private void ResetAll()
		{
			chessboardModel.Reset();
			cpuGameModel.Reset();
			cpuGameModel.inProgress = false;
		}
	}
}