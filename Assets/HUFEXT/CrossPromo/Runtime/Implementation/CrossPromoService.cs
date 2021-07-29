using System.Collections;
using HUF.InitFirebase.Runtime.API;
using HUF.RemoteConfigs.Runtime.API;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUFEXT.CrossPromo.Runtime.Implementation.View.BottomPanel;
using HUFEXT.CrossPromo.Runtime.Implementation.View.ContentPanel;
using HUFEXT.CrossPromo.Runtime.Implementation.View.TopPanel;
using UnityEngine;
using UnityEngine.UI;

namespace HUFEXT.CrossPromo.Runtime.Implementation
{
    public class CrossPromoService
    {
        CrossPromoView crossPromoView;
        TopPanelContainer topPanelContainer;
        ContentPanelContainer contentPanelContainer;
        BottomPanelContainer bottomPanelContainer;
        ScreenOrientation defaultAppOrientation;
        bool isInitialized;
        bool isPanelOpen = false;
        public bool hasContent;
        
        public static string NotInstalledStateButtonText = "get";
        public static string InstalledStateButtonText = "play now";
        public static string InstalledStateTileLabelText = "installed";
        public static string CloseButtonText = "continue to game";

        public CrossPromoService()
        {
            HRemoteConfigs.OnFetchComplete += HandleConfigFetchFinished;
            HRemoteConfigs.OnFetchFail += HandleConfigsFetchFail;

            if ( HRemoteConfigs.IsInitialized() )
            {
                HRemoteConfigs.Fetch();
            }
            else
            {
                HRemoteConfigs.OnInitComplete += HandleRemoteConfigsInitialized;
            }
        }

        /*void HandleFocusLost(bool isActive)
        {
            if (!isActive)
            {
                var config = HConfigs.GetConfig<CrossPromoRemoteConfig>();
                UpdateContainerPanels(config);
            }
        }*/

        public bool IsPanelOpen() => isPanelOpen;

        public void ClosePanel()
        {
            crossPromoView.gameObject.SetActive( false );
            isPanelOpen = false;
            Screen.orientation = defaultAppOrientation;
        }

        public void OpenPanel()
        {
            isPanelOpen = true;
            CoroutineManager.StartCoroutine( InternalOpenPanel() );
        }

        public void SetNotInstalledStateButtonText( string text )
        {
            NotInstalledStateButtonText = text;
            topPanelContainer.UpdateTexts();
            contentPanelContainer.UpdateTexts();
        }

        public void SetInstalledStateButtonText( string text )
        {
            InstalledStateButtonText = text;
            topPanelContainer.UpdateTexts();
            contentPanelContainer.UpdateTexts();
        }

        public void SetInstalledStateTileLabelText( string text )
        {
            InstalledStateTileLabelText = text;
            contentPanelContainer.UpdateTexts();
        }

        public void SetCloseButtonText( string text )
        {
            CloseButtonText = text;
            bottomPanelContainer.UpdateTexts();
        }

        IEnumerator InternalOpenPanel()
        {
            var localConfig = HConfigs.GetConfig<CrossPromoLocalConfig>();

            if ( !localConfig.UseDefaultAppOrientation )
                defaultAppOrientation = Screen.orientation;
            Screen.orientation = ScreenOrientation.Portrait;
            yield return new WaitUntil( () => Screen.orientation == ScreenOrientation.Portrait );

            crossPromoView.gameObject.SetActive( true );
            yield return null;

            LayoutRebuilder.ForceRebuildLayoutImmediate( crossPromoView.GetComponent<RectTransform>() );
        }

        void HandleInitComplete()
        {
            if ( !IsConfigValid() )
            {
                Debug.LogError( "Failed to find at least one of the required configs!" );
                return;
            }

            var localConfig = HConfigs.GetConfig<CrossPromoLocalConfig>();
            var remoteConfig = HConfigs.GetConfig<CrossPromoRemoteConfig>();

            if ( crossPromoView == null )
            {
                if ( localConfig.UseDefaultAppOrientation )
                    defaultAppOrientation = localConfig.DefaultAppOrientation;
                var canvas = CreateCanvas();

                crossPromoView = Object
                    .Instantiate( localConfig.CrossPromoRootPrefab, canvas.transform )
                    .GetComponent<CrossPromoView>();

                if ( localConfig.CustomBottomPanelContainer == null )
                {
                    crossPromoView.BottomPanelContainer =
                        Object.Instantiate( crossPromoView.BottomPanelContainerDefaultPrefab, crossPromoView.transform );
                }
                else
                {
                    crossPromoView.BottomPanelContainer = Object.Instantiate( localConfig.CustomBottomPanelContainer,
                        crossPromoView.transform );
                }

                topPanelContainer = crossPromoView.TopPanelContainer;
                contentPanelContainer = crossPromoView.ContentPanelContainer;
                bottomPanelContainer = crossPromoView.BottomPanelContainer;
                ClosePanel();
            }

            HRemoteConfigs.ApplyConfig( ref remoteConfig );
            isInitialized = true;
            UpdateContainerPanels( remoteConfig );
            hasContent = remoteConfig.CrossPromoPanelGameModels.Count > 0 && remoteConfig.TopPanelCrossPromoGameModels.Count > 0;
        }

        void HandleRemoteConfigsInitialized( RemoteConfigService service )
        {
            HRemoteConfigs.Fetch();
        }

        void HandleConfigsFetchFail( RemoteConfigService? service )
        {
            ApplyConfig();
        }

        void HandleConfigFetchFinished( RemoteConfigService service )
        {
            ApplyConfig();
            HandleInitComplete();
        }

        void ApplyConfig()
        {
            var config = HConfigs.GetConfig<CrossPromoRemoteConfig>();
            HRemoteConfigs.ApplyConfig( ref config );
            UpdateContainerPanels( config );
        }

        void UpdateContainerPanels( CrossPromoRemoteConfig config )
        {
            if ( isInitialized )
            {
                UpdateContentContainerPanel( config );
                UpdateTopContainerPanel( config );
                UpdateBottomContainerPanel( config );
            }
        }

        void UpdateTopContainerPanel( CrossPromoRemoteConfig config )
        {
            var topBanners = config.TopPanelCrossPromoGameModels;
            topPanelContainer.RemoveObsoleteBanners( topBanners );
            topPanelContainer.UpdateOrder();

            foreach ( var topBannerModel in topBanners )
            {
                topPanelContainer.AddNewBanner( topBannerModel );
            }

            topPanelContainer.UpdateBulletPointsColor( config.BulletPointImageColor );
        }

        void UpdateContentContainerPanel( CrossPromoRemoteConfig config )
        {
            var newTilesList = config.CrossPromoPanelGameModels;
            contentPanelContainer.RemoveObsoleteTiles( newTilesList );
            contentPanelContainer.UpdateOrder();

            foreach ( var tile in newTilesList )
            {
                contentPanelContainer.AddOrUpdateGame( tile );
            }
        }

        void UpdateBottomContainerPanel( CrossPromoRemoteConfig config )
        {
            bottomPanelContainer.SetLogoImageSprite( config.BottomPanelLogoSpritePath );
            bottomPanelContainer.SetButtonColor( config.NotInstalledStateButtonColor );
        }

        GameObject CreateCanvas()
        {
            var config = HConfigs.GetConfig<CrossPromoLocalConfig>();
            var crossPromoCanvas = new GameObject( "CrossPromoCanvas" );
            var canvasComponent = crossPromoCanvas.AddComponent<Canvas>();
            canvasComponent.overrideSorting = true;
            canvasComponent.sortingOrder = config.CrossPromoCanvasSortingOrder;
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            var canvasScalerComponent = crossPromoCanvas.AddComponent<CanvasScaler>();
            canvasScalerComponent.referenceResolution = config.BaseResolution;
            canvasScalerComponent.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScalerComponent.matchWidthOrHeight = 1.0f;
            canvasScalerComponent.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            crossPromoCanvas.AddComponent<GraphicRaycaster>();
            return crossPromoCanvas;
        }

        bool IsConfigValid()
        {
            return HConfigs.HasConfig<CrossPromoLocalConfig>()
                   && HConfigs.HasConfig<CrossPromoRemoteConfig>();
        }

        public void FetchRemoteConfigs()
        {
            HandleRemoteConfigsInitialized(RemoteConfigService.Firebase);
            HRemoteConfigs.OnFetchComplete += HandleConfigFetchFinished;
            HRemoteConfigs.OnFetchFail += HandleConfigsFetchFail;
        }

    }
}