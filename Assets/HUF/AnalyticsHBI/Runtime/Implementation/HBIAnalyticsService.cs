using System.Collections.Generic;
using HUF.Analytics.Runtime.API;
using HUF.Analytics.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using HBIAnalytics = huuuge.Analytics;

namespace HUF.AnalyticsHBI.Runtime.Implementation
{
    public class HBIAnalyticsService : IAnalyticsService
    {
        readonly HLogPrefix prefix = new HLogPrefix( nameof( HBIAnalyticsService ) );

        HBIAnalytics hbi;

        public string Name => AnalyticsServiceName.HBI;
        public bool IsInitialized => hbi != null && hbi.IsInitialized();

        public void Init( AnalyticsModel model )
        {
            if ( IsInitialized )
            {
                return;
            }

            var config = HConfigs.GetConfig<HBIAnalyticsConfig>();

            if ( !HasCorrectSettings( config ) )
            {
                model?.CompleteServiceInitialization( Name, false );
                return;
            }

            hbi = new HBIAnalytics( config.ProjectName, config.Sku, Debug.isDebugBuild, config.Amazon ? "amazon" : "" );
            HLog.LogImportant( prefix, $"HBI UserID: {UserId}" );
            model?.CompleteServiceInitialization( Name, IsInitialized );
        }

        public string UserId => IsInitialized ? hbi.UserId() : string.Empty;

        bool HasCorrectSettings( HBIAnalyticsConfig config )
        {
            var configType = typeof(HBIAnalyticsConfig).Name;

            if ( config == null )
            {
                HLog.LogError( prefix, $"Can't find {configType}" );
                return false;
            }

            if ( config.ProjectName.Length > HBIAnalyticsConfig.MaxProjectLength )
            {
                HLog.LogError( prefix, $"Project name is too long - check {configType}" );
                return false;
            }

            if ( config.Sku.Length > HBIAnalyticsConfig.MaxSKULength )
            {
                HLog.LogError( prefix, $"SKU is too long - check {configType}" );
                return false;
            }

            return true;
        }

        public void LogEvent( AnalyticsEvent analyticsEvent )
        {
            if ( !IsInitialized )
            {
                HLog.LogError( prefix, "HBI service isn't initialized." );
                return;
            }

            hbi.LogEvent( GetHBIConvertedParameters( analyticsEvent.EventData ) );
        }

        public void LogMonetizationEvent( AnalyticsMonetizationEvent analyticsEvent )
        {
            if ( !IsInitialized )
            {
                HLog.LogError( prefix, "HBI service isn't initialized." );
                return;
            }

            hbi.LogMonetizeEvent( GetHBIConvertedParameters( analyticsEvent.EventData ), analyticsEvent.Cents );
        }

        public void CollectSensitiveData( bool consentStatus )
        {
            if ( !IsInitialized )
            {
                HLog.LogError( prefix, "HBI service isn't initialized." );
                return;
            }

            hbi.AllowSendSensitiveData( consentStatus );
        }

        Dictionary<string, object> GetHBIConvertedParameters( Dictionary<string, object> input )
        {
            var valueKey = AnalyticsEvent.EventConsts.VALUE_KEY;

            if ( input.ContainsKey( valueKey ) && !IsIntegerLikeType( input[valueKey] ) )
            {
                HLog.LogError( prefix, $" Value parameter could be only int/long/uint type." );
                var output = new Dictionary<string, object>( input );
                output.Remove( valueKey );
                return output;
            }

            return input;
        }

        bool IsIntegerLikeType(object value)
        {
            return value is int || value is long || value is uint;
        }
    }
}