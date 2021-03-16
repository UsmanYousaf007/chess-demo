using HUF.AnalyticsAppsFlyer.Runtime.API;
using HUF.Utils;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Logging;
using UnityEngine;

namespace HUF.AnalyticsAppsFlyer.Runtime.Implementation
{
    public class AppsFlyerTrackerCallbacks : HSingleton<AppsFlyerTrackerCallbacks>
    {
        static readonly HLogPrefix logPrefix =
            new HLogPrefix( HAnalyticsAppsFlyer.logPrefix, nameof(AppsFlyerTrackerCallbacks) );

        void Start()
        {
            HLog.Log( logPrefix, "On Start" );
        }

        public void didReceiveConversionData( string conversionData )
        {
            HLog.Log( logPrefix, $"Got conversion data = {conversionData}" );

            HAnalyticsAppsFlyer.InstallType =
                conversionData.Contains( "Non" ) ? InstallType.NonOrganic : InstallType.Organic;
        }

        public void didReceiveConversionDataWithError( string error )
        {
            HLog.Log( logPrefix, $"Got conversion data error = {error}" );
        }

        public void didFinishValidateReceipt( string validateResult )
        {
            HLog.Log( logPrefix, $"Got didFinishValidateReceipt = {validateResult}" );
        }

        public void didFinishValidateReceiptWithError( string error )
        {
            HLog.Log( logPrefix, $"Got idFinishValidateReceiptWithError error = {error}" );
        }

        public void onAppOpenAttribution( string validateResult )
        {
            HLog.Log( logPrefix, $"Got onAppOpenAttribution = {validateResult}" );
        }

        public void onAppOpenAttributionFailure( string error )
        {
            HLog.Log( logPrefix, $"Got onAppOpenAttributionFailure error = {error}" );
        }

        public void onInAppBillingSuccess()
        {
            HLog.Log( logPrefix, "Got onInAppBillingSuccess success" );
        }

        public void onInAppBillingFailure( string error )
        {
            HLog.Log( logPrefix, $"Got onInAppBillingFailure error = {error}" );
        }

        public void onInviteLinkGenerated( string link )
        {
            HLog.Log( logPrefix, $"Generated userInviteLink {link}" );
        }
    }
}