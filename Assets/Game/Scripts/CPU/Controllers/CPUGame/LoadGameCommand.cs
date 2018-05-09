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
		// Dispatch Signals
		[Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
		[Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
		[Inject] public UpdateMenuViewSignal updateMenuViewSignal { get; set; }

		// Models
		[Inject] public ICPUGameModel cpuGameModel { get; set; }
		[Inject] public IChessboardModel chessboardModel { get; set; }
		[Inject] public IPlayerModel playerModel { get; set; }

		// Services
		[Inject] public ILocalDataService localDataService { get; set; }
		[Inject] public IAdsService adsService { get; set; }

		public override void Execute()
		{
			ResetAll();

			if (!localDataService.FileExists(SaveKeys.CPU_SAVE_FILENAME))
			{
				LogUtil.Log("No saved game or settings found.", "yellow");
				LoadMenu();
				return;
			}

			try
			{
				ILocalDataReader reader = localDataService.OpenReader(SaveKeys.CPU_SAVE_FILENAME);

				// CPU MENU MODEL
				cpuGameModel.totalGames = reader.Read<int>(SaveKeys.TOTAL_GAMES);
				cpuGameModel.cpuStrength = reader.Read<int>(SaveKeys.CPU_STRENGTH);
				cpuGameModel.durationIndex = reader.Read<int>(SaveKeys.DURATION_INDEX);
				cpuGameModel.playerColorIndex = reader.Read<int>(SaveKeys.PLAYER_COLOR_INDEX);
				cpuGameModel.inProgress = reader.Read<bool>(SaveKeys.IN_PROGRESS);

				if (!cpuGameModel.inProgress)
				{
					reader.Close();
					LoadMenu();
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

				reader.Close();

				chessboardEventSignal.Dispatch(ChessboardEvent.GAME_STARTED);
			}
			catch (Exception e)
			{
				LogUtil.Log("Corrupt saved game! " + e, "red");
				localDataService.DeleteFile(SaveKeys.CPU_SAVE_FILENAME);
				ResetAll();
				LoadMenu();
			}
		}

		private void ResetAll()
		{
			chessboardModel.Reset();
			cpuGameModel.Reset();
			cpuGameModel.inProgress = false;
		}

		private void LoadMenu()
		{
			navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MENU);
			updateMenuViewSignal.Dispatch(cpuGameModel.GetCPUMenuVO());
		}
	}
}