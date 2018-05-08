/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using System.Collections.Generic;
using System;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantChess
{
	public class LoadStoreCommand : Command
	{
		// Dispatch Signals
		[Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
		[Inject] public UpdateStoreSignal updateStoreSignal { get; set; }

		// Models
		[Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

		public override void Execute()
		{
			navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_STORE);

			CPUStoreVO vo = new CPUStoreVO();
			vo.storeSettingsModel = storeSettingsModel;

			updateStoreSignal.Dispatch(vo);
		}
	}
}
