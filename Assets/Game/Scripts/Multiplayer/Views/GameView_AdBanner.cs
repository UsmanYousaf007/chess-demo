/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine.UI;
using TurboLabz.Chess;
using UnityEngine;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Inject] public RequestToggleBannerSignal requestToggleBannerSignal { get; set; }
        [Inject] public ToggleBannerSignal toggleBannerSignal { get; set; }

        public Canvas canvas;
        public RectTransform adBanner;

        Vector2 bannerScreenPos;
        bool initComplete;

        public void OnParentShowAdBanner()
        {
            if (!initComplete)
            {
                Vector3[] v = new Vector3[4];
                adBanner.GetWorldCorners(v);
                bannerScreenPos = RectTransformUtility.WorldToScreenPoint(canvas.GetComponent<Camera>(), v[1]);
                bannerScreenPos.y = Screen.currentResolution.height - bannerScreenPos.y;
                initComplete = true;
            }

            requestToggleBannerSignal.Dispatch();
        }

        public void OnParentHideAdBanner()
        {
            toggleBannerSignal.Dispatch(false);
        }

        Rect RectTransformToScreenSpace(RectTransform tfm)
        {
            Vector2 size = Vector2.Scale(tfm.rect.size, tfm.lossyScale);
            return new Rect(tfm.position.x, Screen.height - tfm.position.y, size.x, size.y);
        }
    }
}
