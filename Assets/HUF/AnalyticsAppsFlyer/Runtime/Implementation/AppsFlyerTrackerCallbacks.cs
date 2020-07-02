using HUF.AnalyticsAppsFlyer.Runtime.API;
using HUF.Utils;
using HUF.Utils.Runtime;
using UnityEngine;

namespace HUF.AnalyticsAppsFlyer.Runtime.Implementation
{
    public class AppsFlyerTrackerCallbacks : HSingleton<AppsFlyerTrackerCallbacks>
    {
        readonly string className = typeof(AppsFlyerTrackerCallbacks).Name;

        void Start()
        {
            Debug.Log($"{className} on Start");
        }

        public void didReceiveConversionData(string conversionData)
        {
            Debug.Log($"{className} got conversion data = {conversionData}");
            HAnalyticsAppsFlyer.InstallType = conversionData.Contains("Non") ? 
                InstallType.NonOrganic : 
                InstallType.Organic;
        }

        public void didReceiveConversionDataWithError(string error)
        {
            Debug.Log($"{className} got conversion data error = {error}");
        }

        public void didFinishValidateReceipt(string validateResult)
        {
            Debug.Log($"{className} got didFinishValidateReceipt  = {validateResult}");
        }

        public void didFinishValidateReceiptWithError(string error)
        {
            Debug.Log($"{className} got idFinishValidateReceiptWithError error = {error}");
        }

        public void onAppOpenAttribution(string validateResult)
        {
            Debug.Log($"{className} got onAppOpenAttribution  = {validateResult}");
        }

        public void onAppOpenAttributionFailure(string error)
        {
            Debug.Log($"{className} got onAppOpenAttributionFailure error = {error}");
        }

        public void onInAppBillingSuccess()
        {
            Debug.Log($"{className} got onInAppBillingSuccess success");
        }

        public void onInAppBillingFailure(string error)
        {
            Debug.Log($"{className} got onInAppBillingFailure error = {error}");
        }

        public void onInviteLinkGenerated(string link)
        {
            Debug.Log($"{className} generated userInviteLink {link}");
        }
    }
}