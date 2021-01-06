/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.Chess;
using System;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using TurboLabz.CPU;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ToggleBannerCommand : Command
    {
        // Parameters
        [Inject] public bool enable { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }


        public override void Execute()
        {
            if (playerModel.HasRemoveAds())
            { 
                return;
            }

            var currentState = navigatorModel.currentState.GetType();
            var previousState = navigatorModel.previousState.GetType();
            var canShowBanner = enable &&
                (currentState == typeof(NSMultiplayer) ||
                 currentState == typeof(NSCPU) ||
                (currentState == typeof(NSChat) && previousState == typeof(NSMultiplayer)));

            if (canShowBanner && adsSettingsModel.isBannerEnabled)
            {
                adsService.ShowBanner();
            }
            else
            {
                adsService.HideBanner();
            }
        }
    }
}
