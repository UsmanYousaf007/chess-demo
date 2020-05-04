using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Ads.Runtime.API
{
    public static class HAdsUtils
    {
        static float dpToPxScaleFactor = float.NaN;
        
        /// <summary>
        /// Returns a scale factor between actual screen DPI and 160 DPI screen (used for density independent pixels).
        /// Pixels = DP * DpToPxScaleFactor 
        /// </summary>
        [PublicAPI]
        public static float DpToPxScaleFactor 
        {
            get
            {
                if (float.IsNaN(dpToPxScaleFactor))
                {
                    switch (Application.platform)
                    {
                        case RuntimePlatform.Android:
                            dpToPxScaleFactor = (float) Math.Round(Screen.dpi / 160f, 1);
                            break;
                        case RuntimePlatform.IPhonePlayer:
                            dpToPxScaleFactor = Mathf.RoundToInt(Screen.dpi / 160f);
                            break;
                        default:
                            dpToPxScaleFactor = 1.0f;
                            break;
                    }
                }

                return dpToPxScaleFactor;
            }
        }
        
        /// <summary>
        /// Converts size from DP (density independent pixels) to actual screen pixels
        /// </summary>
        [PublicAPI]
        public static float ConvertDpToPixels(float dp)
        {
            return dp * DpToPxScaleFactor;
        }
        
        /// <summary>
        /// Converts size from actual screen pixels to DP (density independent pixels)
        /// </summary>
        [PublicAPI]
        public static float ConvertPixelsToDp(float pixels)
        {
            return pixels / DpToPxScaleFactor;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void PreCalculateDpToPxScaleFactor()
        {
            var scaleFactor = DpToPxScaleFactor;
        }
    }
}