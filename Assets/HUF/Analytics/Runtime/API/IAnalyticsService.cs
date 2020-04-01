namespace HUF.Analytics.API
{
    public interface IAnalyticsService
    {
        string Name { get; }
        void Init();
        void LogEvent(AnalyticsEvent analyticsEvent);
        void LogMonetizationEvent(AnalyticsMonetizationEvent analyticsEvent);
        void CollectSensitiveData(bool consentStatus);
    }
}