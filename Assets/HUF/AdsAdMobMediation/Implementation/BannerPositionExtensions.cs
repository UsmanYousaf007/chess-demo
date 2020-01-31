using System;
using GoogleMobileAds.Api;
using HUF.Ads.API;

namespace HUF.AdsAdMobMediation.Implementation
{
    public static class BannerPositionExtensions
    {
        public static AdPosition ToAdMobBannerPosition(this BannerPosition position)
        {
            switch (position)
            {
                case BannerPosition.TopLeft:
                    return AdPosition.TopLeft;
                case BannerPosition.TopCenter:
                    return AdPosition.Top;
                case BannerPosition.TopRight:
                    return AdPosition.TopRight;
                case BannerPosition.Centered:
                    return AdPosition.Center;
                case BannerPosition.BottomLeft:
                    return AdPosition.BottomLeft;
                case BannerPosition.BottomCenter:
                    return AdPosition.Bottom;
                case BannerPosition.BottomRight:
                    return AdPosition.BottomRight;
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(position), position, 
                        $"Given banner position {position} is not defined");
                }
            }
        }
    }
}