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
        [Inject] public UpdateTopInventoryBarSignal updateTopInventoryBarSignal { get; set; }

        // Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void Execute()
        {
            HandleAnalytics();

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);

            StoreVO vo = new StoreVO();
            vo.playerModel = playerModel;
            vo.storeSettingsModel = metaDataModel;

            updateSpotPurchaseSignal.Dispatch(vo, activeSection);

            PlayerInventoryVO topInventoryBarVO = new PlayerInventoryVO();
            topInventoryBarVO.safeMoveCount = playerModel.PowerUpSafeMoveCount;
            topInventoryBarVO.hintCount = playerModel.PowerUpHintCount;
            topInventoryBarVO.hindsightCount = playerModel.PowerUpHindsightCount;
            topInventoryBarVO.coinCount = playerModel.bucks;
            updateTopInventoryBarSignal.Dispatch(topInventoryBarVO);
        }

        void HandleAnalytics()
        {
            AnalyticsContext analyticsContext = AnalyticsContext.unknown;

            if (navigatorModel.currentViewId == NavigatorViewId.CPU)
            {
                analyticsContext = AnalyticsContext.computer_match;
            }
            else if (navigatorModel.currentViewId == NavigatorViewId.MULTIPLAYER)
            {
                if (matchInfoModel.activeMatch.isLongPlay)
                {
                    analyticsContext = AnalyticsContext.long_match;
                }
                else
                {
                    analyticsContext = AnalyticsContext.quick_match;
                }
            }

            if (activeSection == SpotPurchaseView.PowerUpSections.SAFEMOVES)
            {
                analyticsService.ScreenVisit(AnalyticsScreen.spot_purchase_safe_move, analyticsContext);
            }
            else if (activeSection == SpotPurchaseView.PowerUpSections.HINTS)
            {
                analyticsService.ScreenVisit(AnalyticsScreen.spot_purchase_hint, analyticsContext);
            }
            else if (activeSection == SpotPurchaseView.PowerUpSections.HINDSIGHTS)
            {
                analyticsService.ScreenVisit(AnalyticsScreen.spot_purchase_hindsight, analyticsContext);
            }
        }
    }
}
