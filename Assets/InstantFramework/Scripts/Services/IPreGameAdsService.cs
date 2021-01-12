using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public interface IPreGameAdsService
    {
        IPromise ShowPreGameAd(string actionCode = null);
    }
}