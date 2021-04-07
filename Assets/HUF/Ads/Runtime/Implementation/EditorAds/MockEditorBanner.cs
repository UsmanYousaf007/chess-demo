using System;
using HUF.Ads.Runtime.API;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.SafeArea;
using HUF.Utils.Runtime.UI.CanvasBlocker;
using UnityEngine;

namespace HUF.Ads.Runtime.Implementation.EditorAds
{
    public class MockEditorBanner : HSingleton<MockEditorBanner>
    {
        const float BANNER_WIDTH_IN_DP = 320;
        const float ADAPTIVE_BANNER_WIDTH_TO_SCREEN_RATIO = 0.8f;
        const float LEADERBOARD_WIDTH_IN_DP = 728;
        const float LEADERBOARD_HEIGHT_IN_DP = 90;
        const string BANNER = "Banner";
        const string LEADERBOARD = "Leaderboard";
        const string ADAPTIVE_WIDTH = "Adaptive width";
        const string CONSTANT_WIDTH = "Constant width";
        const string CHANGE_HEIGHT = "Change height";
        const string CHANGE_COLOR = "Change color";
        readonly int[] bannerHeightsInDp = {50, 70};
        readonly Color firstBannerColor = Color.magenta;
        readonly Color secondBannerColor = Color.clear;

        Color bannerColor;
        bool isLeaderboard = false;
        bool isBannerWidthAdaptive = false;
        int currentBannerHeightId = 0;

        GUIStyle guiStyle;
        Texture2D bannerBackgroundTexture2D;
        BannerPosition bannerAlignment;
        Vector2 bannerPosition;
        Vector2 bannerSize;
        CanvasBlocker canvasBlocker;
        BannerEditorAdsProvider bannerEditorAdsProvider;

        public float HeightInPixels =>
            isLeaderboard ? LEADERBOARD_HEIGHT_IN_DP : bannerHeightsInDp[currentBannerHeightId];

        public void Show( BannerEditorAdsProvider bannerEditorAdsProvider, BannerPosition bannerPosition )
        {
            gameObject.SetActive( true );
            bannerAlignment = bannerPosition;
            this.bannerEditorAdsProvider = bannerEditorAdsProvider;
        }

        public void Hide()
        {
            gameObject.SetActive( false );
            canvasBlocker.Hide();
        }

        static float ToHorizontalPosition( BannerPosition bannerPosition, float bannerWidth )
        {
            switch ( bannerPosition )
            {
                case BannerPosition.BottomLeft:
                case BannerPosition.TopLeft:
                    return 0;
                case BannerPosition.BottomRight:
                case BannerPosition.TopRight:
                    return ScreenSize.Width - bannerWidth;
                default:
                    return ( ScreenSize.Width - bannerWidth ) / 2;
            }
        }

        static float ToVerticalPosition( BannerPosition bannerPosition, float bannerHeight )
        {
            switch ( bannerPosition )
            {
                case BannerPosition.TopLeft:
                case BannerPosition.TopCenter:
                case BannerPosition.TopRight:
                    return 0;
                case BannerPosition.Centered:
                    return ( ScreenSize.Height - bannerHeight ) / 2;
                default:
                    return ScreenSize.Height - bannerHeight;
            }
        }

        void BannerIsRefreshed()
        {
            bannerEditorAdsProvider.BannerIsRefreshed();
        }

        void Awake()
        {
            bannerColor = firstBannerColor;
            canvasBlocker = CanvasBlocker.Create( nameof(MockEditorBanner) );
            bannerBackgroundTexture2D = new Texture2D( 1, 1 );
            bannerBackgroundTexture2D.SetPixel( 0, 0, bannerColor );
            bannerBackgroundTexture2D.Apply();
        }

        void Update()
        {
            canvasBlocker.ShowPanel( bannerColor, bannerPosition, bannerSize );
        }

        void OnGUI()
        {
            GUI.skin.button.wordWrap = true;

            if ( guiStyle == null )
            {
                guiStyle = new GUIStyle( GUI.skin.box );
                guiStyle.normal.background = bannerBackgroundTexture2D;
            }

            float bannerWidth = 0;
            float bannerHeight = HAdsUtils.ConvertDpToPixels( HeightInPixels );

            if ( isLeaderboard )
            {
                bannerWidth = HAdsUtils.ConvertDpToPixels( LEADERBOARD_WIDTH_IN_DP );
            }
            else
            {
                if ( isBannerWidthAdaptive )
                    bannerWidth = ADAPTIVE_BANNER_WIDTH_TO_SCREEN_RATIO * ScreenSize.Width;
                else
                    bannerWidth = HAdsUtils.ConvertDpToPixels( BANNER_WIDTH_IN_DP );
            }

            float xPosition = ToHorizontalPosition( bannerAlignment, bannerWidth );
            float yPosition = ToVerticalPosition( bannerAlignment, bannerHeight );
            bannerPosition = new Vector2( xPosition, yPosition );
            bannerSize = new Vector2( bannerWidth, bannerHeight );
            GUI.Box( new Rect( bannerPosition, bannerSize ), string.Empty, guiStyle );
            float buttonWidth = bannerWidth / 4;
            Vector2 buttonSize = new Vector2( buttonWidth, bannerHeight );
            Vector2 buttonPosition = bannerPosition;

            if ( isLeaderboard )
            {
                if ( GUI.Button( new Rect( buttonPosition,
                        new Vector2( bannerWidth - buttonWidth, bannerHeight ) ),
                    LEADERBOARD ) )
                {
                    isLeaderboard = false;
                    BannerIsRefreshed();
                }

                buttonPosition.x += bannerWidth - buttonWidth;
            }
            else
            {
                if ( GUI.Button( new Rect( buttonPosition, buttonSize ),
                    BANNER ) )
                {
                    isLeaderboard = true;
                    BannerIsRefreshed();
                }

                buttonPosition.x += buttonWidth;

                if ( GUI.Button( new Rect( buttonPosition, buttonSize ),
                    isBannerWidthAdaptive ? ADAPTIVE_WIDTH : CONSTANT_WIDTH ) )
                {
                    isBannerWidthAdaptive = !isBannerWidthAdaptive;
                    BannerIsRefreshed();
                }

                buttonPosition.x += buttonWidth;

                if ( GUI.Button( new Rect( buttonPosition, buttonSize ), CHANGE_HEIGHT ) )
                {
                    currentBannerHeightId = ( currentBannerHeightId + 1 ) % bannerHeightsInDp.Length;
                    BannerIsRefreshed();
                }

                buttonPosition.x += buttonWidth;
            }

            if ( GUI.Button( new Rect( buttonPosition, buttonSize ), CHANGE_COLOR ) )
            {
                if ( bannerColor == firstBannerColor )
                    bannerColor = secondBannerColor;
                else
                    bannerColor = firstBannerColor;
                bannerBackgroundTexture2D.SetPixel( 0, 0, bannerColor );
                bannerBackgroundTexture2D.Apply();
                BannerIsRefreshed();
            }
        }
    }
}