using System.Collections;
using System.Collections.Generic;
using System.Text;
using HUF.Analytics.Runtime.API;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using UnityEngine;

namespace HUF.Analytics.Runtime.Implementation
{
    public class CacheService
    {
        const int CACHE_CAPACITY = 25;
        const int APPSFLYER_CACHE_CAPACITY = 5;
        const string SAVE_KEY = "HUFAnalyticsCache_";
        const char EVENTS_SEPARATOR = '\x01';
        const string SAVE_COUNT_STRING_FORMAT = SAVE_KEY + "{0}_Count";

        static readonly HLogPrefix logPrefix = new HLogPrefix( HAnalytics.prefix, nameof(CacheService) );

        readonly string serviceName;
        readonly List<AnalyticsEvent> listOfEvents = new List<AnalyticsEvent>();
        readonly int cacheCapacity;

        int savedPagesCount;
        bool isSendingCachedEvents;

        public CacheService( string inServiceName )
        {
            serviceName = inServiceName;

            cacheCapacity = serviceName == AnalyticsServiceName.APPS_FLYER ? APPSFLYER_CACHE_CAPACITY : CACHE_CAPACITY;

            savedPagesCount = HPlayerPrefs.GetInt( string.Format( SAVE_COUNT_STRING_FORMAT, serviceName ) );
            PauseManager.Instance.OnApplicationFocusChange += HandleApplicationFocusChange;
        }

        void HandleApplicationFocusChange( bool hasFocus )
        {
            if ( hasFocus )
                return;

            SaveEvents( false );
        }

        public void AddEvent( AnalyticsEvent analyticsEvent )
        {
            listOfEvents.Add( analyticsEvent );

            if ( listOfEvents.Count == cacheCapacity )
            {
                SaveEvents();
            }
        }

        public void SendEvents( float delay )
        {
            if ( isSendingCachedEvents )
                return;

            isSendingCachedEvents = true;
            CoroutineManager.StartCoroutine( SendEventsCoroutine( delay ) );
        }

        IEnumerator SendEventsCoroutine( float delay )
        {
            WaitForSecondsRealtime delayNextEvents = new WaitForSecondsRealtime( 0.1f );
            yield return new WaitForSecondsRealtime( 1f + delay );

            SaveEvents();
            yield return null;

            for ( int i = 0; i < savedPagesCount; i++ )
            {
                string currentIndexKey = GetIndexedServiceKey( serviceName, i );
                string serializedList = HPlayerPrefs.GetString( currentIndexKey );

                if ( serializedList.IsNullOrEmpty() )
                {
                    continue;
                }

                var listOfSerializedEvents = serializedList.Split( EVENTS_SEPARATOR );
                HLog.Log( logPrefix, $"Log {listOfSerializedEvents.Length} cached events to service {serviceName}" );

                foreach ( var singleEvent in listOfSerializedEvents )
                {
                    if ( singleEvent.Contains( AnalyticsMonetizationEvent.CENTS_KEY ) )
                    {
                        var eventData = (Dictionary<string, object>)HUFJson.Deserialize( singleEvent );

                        HAnalytics.LogMonetizationEvent(
                            AnalyticsMonetizationEvent.Create( eventData,
                                int.Parse( eventData[AnalyticsMonetizationEvent.CENTS_KEY].ToString() ) ),
                            serviceName );
                    }
                    else
                    {
                        HAnalytics.LogEvent(
                            new AnalyticsEvent( (Dictionary<string, object>)HUFJson.Deserialize( singleEvent ) ),
                            serviceName );
                    }
                }

                HPlayerPrefs.DeleteKey( currentIndexKey );
                yield return delayNextEvents;
            }

            savedPagesCount = 0;
            HPlayerPrefs.DeleteKey( string.Format( SAVE_COUNT_STRING_FORMAT, serviceName ) );
        }

        void SaveEvents( bool createNewPage = true )
        {
            if ( listOfEvents.Count == 0 )
                return;

            var serializedData = new StringBuilder();

            foreach ( var singleEvent in listOfEvents )
            {
                serializedData.Append( HUFJson.Serialize( singleEvent.EventData ) );
                serializedData.Append( EVENTS_SEPARATOR );
            }

            HPlayerPrefs.SetString( GetIndexedServiceKey( serviceName, savedPagesCount ),
                serializedData.ToString().TrimEnd( EVENTS_SEPARATOR ) );
            HPlayerPrefs.SetInt( string.Format( SAVE_COUNT_STRING_FORMAT, serviceName ), savedPagesCount + 1 );

            if ( !createNewPage )
                return;

            listOfEvents.Clear();
            savedPagesCount++;
        }

        static string GetIndexedServiceKey( string serviceName, int index )
        {
            return $"{SAVE_KEY}{serviceName}_{index}";
        }
    }
}