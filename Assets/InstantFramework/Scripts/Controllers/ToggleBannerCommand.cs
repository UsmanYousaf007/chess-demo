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
        [Inject] public Vector2 position { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        public override void Execute()
        {
            if (playerModel.HasRemoveAds(metaDataModel.adsSettings))
            { 
                return;
            }

            if (!enable)
            {
                adsService.HideBanner();
            }
            else
            {
                adsService.ShowBanner((int)position.x, (int)position.y);
            }
        }
    }
}
