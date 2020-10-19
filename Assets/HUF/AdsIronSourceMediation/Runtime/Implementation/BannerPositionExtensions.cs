using System;
using HUF.Ads.Runtime.API;
using HUF.AdsIronSourceMediation.Runtime.API;
using HUF.Utils.Runtime.Logging;

namespace HUF.AdsIronSourceMediation.Runtime.Implementation
{
    public static class BannerPositionExtensions
    {
        static readonly HLogPrefix logPrefix =
            new HLogPrefix( HAdsIronSourceMediation.logPrefix, nameof(BannerPositionExtensions) );

        public static IronSourceBannerPosition ToIronSourceBannerPosition( this BannerPosition position )
        {
            switch ( position )
            {
                case BannerPosition.TopLeft:
                    return IronSourceBannerPosition.TOP;
                case BannerPosition.TopCenter:
                    return IronSourceBannerPosition.TOP;
                case BannerPosition.TopRight:
                    return IronSourceBannerPosition.TOP;
                case BannerPosition.Centered:
                {
                    LogNotSupportedPosition( position );
                    return IronSourceBannerPosition.BOTTOM;
                }
                case BannerPosition.BottomLeft:
                    return IronSourceBannerPosition.BOTTOM;
                case BannerPosition.BottomCenter:
                    return IronSourceBannerPosition.BOTTOM;
                case BannerPosition.BottomRight:
                    return IronSourceBannerPosition.BOTTOM;
                default:
                {
                    LogNotSupportedPosition( position );
                    return IronSourceBannerPosition.BOTTOM;
                }
            }
        }

        static void LogNotSupportedPosition( BannerPosition position )
        {
            HLog.LogError( logPrefix, $"Banner position: {position} is not supported." );
        }
    }
}