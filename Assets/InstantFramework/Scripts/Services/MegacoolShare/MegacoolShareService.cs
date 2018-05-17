/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-10 10:52:49 UTC+05:00

using UnityEngine;
using System;

namespace TurboLabz.InstantFramework
{
    public class MegacoolShareService : IShareService
    {
        [Inject] public IAnalyticsService analyticsService { get; set; }

        private const string APP_PROMO_IMAGE = "Share.png";

        public void Init()
        {
            Megacool.Instance.Start();

            Megacool.Instance.CompletedSharing += () => {
                analyticsService.SocialShareComplete();
            };
            Megacool.Instance.DismissedSharing += () => {
                analyticsService.SocialShareDismiss();
            };
            Megacool.Instance.PossiblyCompletedSharing += () => {
                analyticsService.SocialShareUnknown();
            };
        }

        public void ShareApp(string message)
        {
            Megacool.Instance.SharingText = message;
            Megacool.Instance.Share(new MegacoolShareConfig {
                FallbackImage = APP_PROMO_IMAGE,
                Url = new Uri("?_autoopen=true", UriKind.Relative)
            });
        }
    }
}