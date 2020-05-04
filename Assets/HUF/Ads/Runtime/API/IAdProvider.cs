namespace HUF.Ads.Runtime.API
{
    public interface IAdProvider
    {
        string ProviderId { get; }
        bool IsInitialized { get; }

        bool Init();
        void CollectSensitiveData(bool consentStatus);
    }
}