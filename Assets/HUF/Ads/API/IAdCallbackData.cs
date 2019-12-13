using HUF.Ads.Implementation;

namespace HUF.Ads.API
{
    public interface IAdCallbackData : IBaseAdCallbackData
    {
        AdResult Result { get; }
    }
}