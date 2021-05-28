using System.Collections.Generic;
using AppsFlyerSDK;
using HUF.AnalyticsAppsFlyer.Runtime.API;
using HUF.Utils;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;

namespace HUF.AnalyticsAppsFlyer.Runtime.Implementation
{
    public class AppsFlyerTrackerCallbacks : HSingleton<AppsFlyerTrackerCallbacks>,
        IAppsFlyerConversionData,
        IAppsFlyerValidateReceipt,
        IAppsFlyerUserInvite
    {
        const string INSTALL_TYPE_KEY = "af_status";

        static readonly HLogPrefix logPrefix =
            new HLogPrefix( HAnalyticsAppsFlyer.logPrefix, nameof(AppsFlyerTrackerCallbacks) );

        void Start()
        {
            HLog.Log( logPrefix, "On Start" );
        }

        public void didFinishValidateReceipt( string validateResult )
        {
            HLog.Log( logPrefix, $"Got didFinishValidateReceipt = {validateResult}" );
        }

        public void didFinishValidateReceiptWithError( string error )
        {
            HLog.Log( logPrefix, $"Got idFinishValidateReceiptWithError error = {error}" );
        }

        public void onConversionDataSuccess( string conversionData )
        {
            HLog.Log( logPrefix, $"Got conversion data = {conversionData}" );

            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);

            if ( conversionDataDictionary.TryGetValue( INSTALL_TYPE_KEY, out var installTypeObj ) )
            {
                var installTypeString = installTypeObj.ToString();

                if ( !installTypeString.IsNullOrEmpty() )
                {
                    switch ( installTypeString )
                    {
                        case "Non-organic":
                            HAnalyticsAppsFlyer.InstallType = InstallType.NonOrganic;
                            break;
                        case "organic":
                            HAnalyticsAppsFlyer.InstallType = InstallType.Organic;
                            break;
                        default:
                            HAnalyticsAppsFlyer.InstallType = InstallType.NotSpecified;
                            break;
                    }
                }
            }
        }

        public void onConversionDataFail( string error )
        {
            HLog.Log( logPrefix, $"Got conversion data error = {error}" );
        }

        public void onAppOpenAttribution( string validateResult )
        {
            HLog.Log( logPrefix, $"Got onAppOpenAttribution = {validateResult}" );
        }

        public void onAppOpenAttributionFailure( string error )
        {
            HLog.Log( logPrefix, $"Got onAppOpenAttributionFailure error = {error}" );
        }

        public void onInviteLinkGenerated( string link )
        {
            HLog.Log( logPrefix, $"Generated userInviteLink {link}" );
        }

        public void onInviteLinkGeneratedFailure( string error )
        {
            HLog.Log( logPrefix, $"userInviteLink generation failure, error: {error}" );
        }

        public void onOpenStoreLinkGenerated( string link )
        {
            HLog.Log( logPrefix, $"Generated Open Store Link {link}" );
        }

        public void onInAppBillingSuccess()
        {
            HLog.Log( logPrefix, "Got onInAppBillingSuccess success" );
        }

        public void onInAppBillingFailure( string error )
        {
            HLog.Log( logPrefix, $"Got onInAppBillingFailure error = {error}" );
        }
    }
}