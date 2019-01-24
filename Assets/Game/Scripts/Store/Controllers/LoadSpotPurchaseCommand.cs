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

namespace TurboLabz.InstantGame
{
    public class LoadSpotPurchaseCommand : Command
    {
        [Inject] public SpotPurchaseView.PowerUpSections activeSection { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateSpotPurchaseSignal updateSpotPurchaseSignal { get; set; }

        // Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);

            StoreVO vo = new StoreVO();
            vo.playerModel = playerModel;
            vo.storeSettingsModel = metaDataModel;

            updateSpotPurchaseSignal.Dispatch(vo, activeSection);
        }
    }
}
