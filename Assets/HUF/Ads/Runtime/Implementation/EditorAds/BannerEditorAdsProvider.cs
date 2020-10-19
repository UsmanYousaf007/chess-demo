using HUF.Ads.Runtime.API;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Ads.Runtime.Implementation.EditorAds
{
    public class BannerEditorAdsProvider : IBannerAdProvider
    {
        readonly HLogPrefix logPrefix = new HLogPrefix( nameof(BannerEditorAdsProvider) );
        public string ProviderId => "EditorAds";
        public bool IsInitialized => true;

        public event UnityAction<IBannerCallbackData, bool> OnBannerShown;
        public event UnityAction<IBannerCallbackData> OnBannerFailed;
        public event UnityAction<IBannerCallbackData> OnBannerClicked;
        public event UnityAction<IBannerCallbackData> OnBannerHidden;

        public bool Init()
        {
            HLog.Log( logPrefix, $"Initialized Editor Interstitial ads provider" );
            return true;
        }

        public void CollectSensitiveData( bool consentStatus )
        {
            HLog.Log( logPrefix, $"Collect sensitive data: {consentStatus}" );
        }

        public bool Show( BannerPosition position = BannerPosition.BottomCenter )
        {
            MockEditorBanner.Instance.Show( position );
            return true;
        }

        public bool Show( string placementId, BannerPosition position = BannerPosition.BottomCenter )
        {
            MockEditorBanner.Instance.Show( position );
            return true;
        }

        public void Hide()
        {
            MockEditorBanner.Instance.Hide();
        }
    }
}