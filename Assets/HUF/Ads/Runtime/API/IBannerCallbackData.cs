using System;

namespace HUF.Ads.API
{
    public interface IBannerCallbackData : IBaseAdCallbackData
    {
        float HeightInPx { get; }
        float HeightInDp { get; }
        
        [Obsolete("Use `HeightInPx` or `HeightInDp` instead.", true)]
        float Height { get; }
    }
}