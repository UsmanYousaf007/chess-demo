using System.Collections.Generic;
using System.Linq;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.UI.CanvasBlocker;

namespace HUF.Ads.Runtime.Implementation.EditorAds
{
    public abstract class BaseEditorAdProvider
    {
        protected abstract HLogPrefix LogPrefix { get; }

        public string ProviderId => "EditorAds";
        public bool IsInitialized => true;

        readonly HashSet<string> fetchedPlacements = new HashSet<string>();
        protected string lastShownPlacement;
        protected string lastFetchedPlacement;
        protected PlacementType placementType;

        public virtual bool Init()
        {
            HLog.Log( LogPrefix, "Initialized Editor ads provider" );
            return true;
        }

        public void CollectSensitiveData( bool consentStatus )
        {
            HLog.Log( LogPrefix, $"Collect sensitive data: {consentStatus}" );
        }

        public bool IsReady()
        {
            return IsReady( GetPlacementId() );
        }

        public bool IsReady( string placementId )
        {
            HLog.Log( LogPrefix, "Is ready" );
            return fetchedPlacements.Contains( placementId );
        }

        public bool Show()
        {
            return Show( GetPlacementId() );
        }

        public bool Show( string placementId )
        {
#if !HUFEXT_ADS_MANAGER
            if ( !fetchedPlacements.Contains( placementId ) )
                return false;
#endif

            lastShownPlacement = placementId;
            AddDebugScreenButton( Implementation.AdResult.Failed );
            AddDebugScreenButton( Implementation.AdResult.Skipped );
            AddDebugScreenButton( Implementation.AdResult.Completed );
            DebugButtonsScreen.Instance.Show( "Ad Debug Screen" );
            return true;
        }

        void AddDebugScreenButton( Implementation.AdResult adResult )
        {
            DebugButtonsScreen.Instance.AddGUIButton( $"Result:{adResult}", () => HandleAdResult( adResult ) );
        }

        public virtual void Fetch()
        {
            lastFetchedPlacement = GetPlacementId();
            fetchedPlacements.Add( lastFetchedPlacement );
        }

        public virtual void Fetch( string placementId )
        {
            lastFetchedPlacement = placementId;
            fetchedPlacements.Add( placementId );
        }

        protected abstract void OnAdResult( AdResult adResult );

        void HandleAdResult( AdResult adResult )
        {
            DebugButtonsScreen.Instance.Hide();
            OnAdResult( adResult );
            lastShownPlacement = string.Empty;
        }

        string GetPlacementId()
        {
            var adPlacementData =
                HConfigs.GetConfigsByBaseClass<AdsProviderConfig>().FirstOrDefault()?.AdPlacementData;
            var placement = adPlacementData?.FirstOrDefault( x => x.PlacementType == placementType );
            return placement != null ? placement.PlacementId : string.Empty;
        }
    }
}