using HUF.Analytics.Runtime.API;
using HUF.Utils.Runtime.Logging;

namespace HUF.Analytics.Runtime.Implementation
{
    public class DummyAnalyticsService : IAnalyticsService
    {
        public string Name => $"{AnalyticsServiceName.DUMMY}_{NameSuffix}";
        public bool IsInitialized => true;
        public string NameSuffix { get; }
        public string UserId => string.Empty;
        readonly HLogPrefix logPrefix;

        DummyAnalyticsService( string nameSuffix )
        {
            NameSuffix = nameSuffix;
            logPrefix = new HLogPrefix( HAnalytics.prefix, Name );
        }

        public void Init( AnalyticsModel model )
        {
            model?.CompleteServiceInitialization( Name, IsInitialized );
        }

        public void LogEvent( AnalyticsEvent analyticsEvent )
        {
            HLog.Log( logPrefix, $"Log event {analyticsEvent.EventName}" );
        }

        public void LogMonetizationEvent( AnalyticsMonetizationEvent analyticsEvent )
        {
            HLog.Log( logPrefix, $"Log monetization event {analyticsEvent.EventName}" );
        }

        public void CollectSensitiveData( bool consentStatus )
        {
            HLog.Log( logPrefix, $"CollectSensitiveData {consentStatus.ToString()}" );
        }
    }
}