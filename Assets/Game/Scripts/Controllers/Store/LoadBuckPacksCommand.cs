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
	public class LoadBuckPacksCommand : Command
	{
		// Dispatch Signals
		[Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
		[Inject] public UpdateStoreBuckPacksDlgSignal updateStoreBuckPacksDlgSignal { get; set; }

		// Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }
		[Inject] public IPlayerModel playerModel { get; set; }

		public override void Execute()
		{
			navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_BUCK_PACKS_DLG);

			StoreVO vo = new StoreVO();
			vo.storeSettingsModel = metaDataModel;
			vo.playerModel = playerModel;

			updateStoreBuckPacksDlgSignal.Dispatch(vo);
		}
	}
}
