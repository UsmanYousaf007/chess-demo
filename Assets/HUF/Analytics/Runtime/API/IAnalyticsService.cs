using HUF.Analytics.Runtime.Implementation;

namespace HUF.Analytics.Runtime.API
{
    public interface IAnalyticsService
    {
        string Name { get; }
        bool IsInitialized { get; }
        void Init( AnalyticsModel model );
        void LogEvent( AnalyticsEvent analyticsEvent );
        void LogMonetizationEvent( AnalyticsMonetizationEvent analyticsEvent );
        void CollectSensitiveData( bool consentStatus );
    }
}