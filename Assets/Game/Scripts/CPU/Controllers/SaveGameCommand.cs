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
using TurboLabz.TLUtils;
using System;
using UnityEngine;
using TurboLabz.InstantGame;


namespace TurboLabz.CPU
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
				writer.Write<int>(SaveKeys.TOTAL_GAMES, cpuGameModel.totalGames);
				writer.Write<int>(SaveKeys.CPU_STRENGTH, cpuGameModel.cpuStrength);
				writer.Write<bool>(SaveKeys.IN_PROGRESS, cpuGameModel.inProgress);

                LogUtil.Log("SAVING GAME... game in progress:" + cpuGameModel.inProgress, "red");

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
				writer.Write<bool>(SaveKeys.USED_HELP, chessboardModel.usedHelp);
				writer.Write<bool>(SaveKeys.POWER_MODE, chessboardModel.powerMode);
				writer.Write<int>(SaveKeys.FREE_HINTS, chessboardModel.freeHints);

				List<string> moveListJson = new List<string>();

				foreach (ChessMove move in chessboardModel.moveList)
				{
					moveListJson.Add(JsonUtility.ToJson(move));
				}

				writer.WriteList<string>(SaveKeys.MOVE_LIST, moveListJson);

                #region Step System Trimmed Move List

                List<string> trimmedMoveListJson = new List<string>();

                foreach (ChessMove trimmedMove in chessboardModel.trimmedMoveList)
                {
                    trimmedMoveListJson.Add(JsonUtility.ToJson(trimmedMove));
                }

                writer.WriteList<string>(SaveKeys.TRIMMED_MOVE_LIST, trimmedMoveListJson);

                #endregion

                writer.Close();
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