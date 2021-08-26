using System;
using System.Collections.Generic;
using System.Threading;
using HUF.Analytics.Runtime.API;
using HUF.Analytics.Runtime.Implementation;
using HUF.AnalyticsHBI.Runtime.API;
using HUF.PolicyGuard.Runtime.API;
using HUF.PolicyGuard.Runtime.Implementations;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using UnityEngine;
using HBIAnalytics = huuuge.Analytics;

namespace HUF.AnalyticsHBI.Runtime.Implementation
{
    public class HBIAnalyticsService : IAnalyticsService
    {
        const string HUF_HDS_FIRST_LAUNCH_EVENT = nameof(HUF_HDS_FIRST_LAUNCH_EVENT);
        static readonly HLogPrefix logPrefix = new HLogPrefix( HAnalyticsHBI.logPrefix, nameof(HBIAnalyticsService) );

        HBIAnalytics hbi;
        static SynchronizationContext syncContext;

        public string Name => AnalyticsServiceName.HBI;
        public bool IsInitialized => hbi != null && hbi.IsInitialized();
        public string UserId => IsInitialized ? hbi.UserId() : string.Empty;
        public string SessionId => IsInitialized ? hbi.SessionId() : string.Empty;

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

            PauseManager.Instance.OnApplicationFocusChange += HandleApplicationFocus;
#if UNITY_IOS
            HPolicyGuard.OnATTConsentChanged += status =>
            {
                if ( status != AppTrackingTransparencyBridge.AuthorizationStatus.NotDetermined )
                    hbi.UpdateTrackingConsent();
            };
#endif
            hbi = new HBIAnalytics( config.ProjectName, config.Sku, Debug.isDebugBuild,
                config.CustomPlatformName.IsNullOrEmpty() ? string.Empty : config.CustomPlatformName );
            syncContext = SynchronizationContext.Current;
            HLog.LogImportant( logPrefix, $"UserID: {UserId} | SessionID: {SessionId}" );
            model?.CompleteServiceInitialization( Name, IsInitialized );
            Application.quitting += Dispose;
            TrySendFirstLaunchEvent();
        }

        public void LogEvent( AnalyticsEvent analyticsEvent )
        {
            if ( !IsInitialized )
            {
                HLog.LogError( logPrefix, "HBI service isn't initialized." );
                return;
            }

            hbi.LogEvent( GetHBIConvertedParameters( analyticsEvent.EventData ) );
        }

        public void LogMonetizationEvent( AnalyticsMonetizationEvent analyticsEvent )
        {
            if ( !IsInitialized )
            {
                HLog.LogError( logPrefix, "HBI service isn't initialized." );
                return;
            }

            hbi.LogMonetizeEvent( GetHBIConvertedParameters( analyticsEvent.EventData ), analyticsEvent.Cents );
        }

        public void CollectSensitiveData( bool consentStatus )
        {
            if ( !IsInitialized )
            {
                HLog.LogError( logPrefix, "HBI service isn't initialized." );
                return;
            }

            hbi.AllowSendSensitiveData( consentStatus );
        }

        public void SetMinutesToResetSessionId( int minutes )
        {
            HAnalyticsHBI.minutesToResetSessionId = minutes;
        }

        Dictionary<string, object> GetHBIConvertedParameters( Dictionary<string, object> input )
        {
            const string valueKey = AnalyticsEvent.EventConsts.VALUE_KEY;
            var output = new Dictionary<string, object>( input );

            if ( input.ContainsKey( valueKey ) && !IsIntegerLikeType( input[valueKey] ) )
            {
                HLog.LogError( logPrefix, "Value parameter could be only int/long/uint type." );
                output.Remove( valueKey );
            }

            foreach ( var dic in input )
            {
                switch ( dic.Value )
                {
                    case null:
                        HLog.LogError( logPrefix, $"Parameter {dic.Key} contains a null value, removing..." );
                        output.Remove( dic.Key );
                        break;
                    case bool _:
                    case int _:
                    case long _:
                        continue;
                    case double d:
                        if ( double.IsNaN( d ) )
                        {
                            HLog.LogError( logPrefix,
                                $"Parameter {dic.Key} of double type contains a NaN value, removing..." );
                            output.Remove( dic.Key );
                        }

                        break;
                    case string s:
                        if ( string.IsNullOrEmpty( s ) )
                        {
                            HLog.LogError( logPrefix,
                                $"Parameter {dic.Key} of string type contains a null or empty value, removing..." );
                            output.Remove( dic.Key );
                        }

                        break;
                    case float f:
                        if ( float.IsNaN( f ) )
                        {
                            HLog.LogError( logPrefix,
                                $"Parameter {dic.Key} of double type contains a NaN value, removing..." );
                            output.Remove( dic.Key );
                            break;
                        }

                        output[dic.Key] = Convert.ToDouble( dic.Value );
                        break;
                    case uint _:
                        output[dic.Key] = Convert.ToInt64( dic.Value );
                        break;
                    default:
                    {
                        HLog.LogWarning( logPrefix,
                            $"Parameter {dic.Key} is of unsupported type, converting to string..." );
                        var toString = dic.Value.ToString();

                        if ( toString.IsNullOrEmpty() )
                            output.Remove( dic.Key );
                        else
                            output[dic.Key] = toString;
                        break;
                    }
                }
            }

            return output;
        }

        bool IsIntegerLikeType( object value )
        {
            return value is int || value is long || value is uint;
        }

        void HandleApplicationFocus( bool pauseStatus )
        {
            if ( pauseStatus && HAnalyticsHBI.minutesToResetSessionId != 0 )
            {
                hbi.ResetSession( HAnalyticsHBI.minutesToResetSessionId );
                HLog.LogImportant( logPrefix, $"Focus restored, SessionID: {SessionId}" );
            }
        }

        void Dispose()
        {
            syncContext.Post( data => hbi.Dispose(), null );
        }

        bool HasCorrectSettings( HBIAnalyticsConfig config )
        {
            string configType = nameof(HBIAnalyticsConfig);

            if ( config == null )
            {
                HLog.LogError( logPrefix, $"Can't find {configType}" );
                return false;
            }

            if ( config.ProjectName.Length > HBIAnalyticsConfig.MaxProjectLength )
            {
                HLog.LogError( logPrefix, $"Project name is too long - check {configType}" );
                return false;
            }

            if ( config.Sku.Length <= HBIAnalyticsConfig.MaxSKULength )
                return true;

            HLog.LogError( logPrefix, $"SKU is too long - check {configType}" );
            return false;
        }

        void TrySendFirstLaunchEvent()
        {
            if ( HPlayerPrefs.HasKey( HUF_HDS_FIRST_LAUNCH_EVENT ) || HAnalytics.GetGDPRConsent() != null)
                return;

            LogEvent( new AnalyticsEvent( "first_launch" ).ST1( "launch" ));
            HPlayerPrefs.SetBool( HUF_HDS_FIRST_LAUNCH_EVENT, true );
        }
    }
}