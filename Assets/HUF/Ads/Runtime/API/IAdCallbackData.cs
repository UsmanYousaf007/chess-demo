using HUF.Ads.Runtime.Implementation;

namespace HUF.Ads.Runtime.API
{
    public interface IAdCallbackData : IBaseAdCallbackData
    {
        AdResult Result { get; }
    }
}