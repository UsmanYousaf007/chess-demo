namespace HUF.Ads.API
{
    public interface IAdProvider
    {
        string ProviderId { get; }
        bool IsInitialized { get; }

        bool Init();
        void CollectSensitiveData(bool consentStatus);
    }
}