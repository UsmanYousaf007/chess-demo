/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using UnityEngine;
using TurboLabz.InstantFramework;
using System.Collections.Generic;

namespace TurboLabz.InstantChess
{
	public class AdjustThemeCommand : Command
	{
		// Dispatch Signal
		[Inject] public UpdateThemeSignal updateThemeSignal { get; set; }
		[Inject] public UpdateSkinSignal updateSkinSignal { get; set; }
		[Inject] public ApplySkinSignal applySkinSignal { get; set; }

		// Parameters
		[Inject] public bool increase { get; set; }

		// Models
		[Inject] public IPlayerModel playerModel { get; set; }
		[Inject] public IMetaDataModel metaDataModel  { get; set; }
		[Inject] public ICPUGameModel cpuGameModel { get; set; }

		public override void Execute()
		{
			List<string> list = playerModel.vGoods;
			int index = list.IndexOf(playerModel.activeSkinId);

			// No cycling
			if ((increase && (index == (list.Count-1))) ||
				(!increase && (index == 0))) 
			{
				return;
			}

			playerModel.activeSkinId = increase ? list[index + 1] : list[index - 1];

			StoreItem item = metaDataModel.items[playerModel.activeSkinId];

			applySkinSignal.Dispatch(playerModel.activeSkinId);
			updateSkinSignal.Dispatch();

			CPULobbyVO vo = new CPULobbyVO(cpuGameModel, playerModel, metaDataModel);
			updateThemeSignal.Dispatch(vo);
		}
	}
}
