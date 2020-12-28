using System;
using System.Collections.Generic;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using HUFEXT.GenericGDPR.Runtime.API;
using HUFEXT.GenericGDPR.Runtime.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TurboLabz.InstantFramework;

namespace HUFEXT.GenericGDPR.Runtime.Views
{
    public class GDPRView : MonoBehaviour
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof( GDPRView ) );

        [Serializable]
        public class ColorScheme
        {
            public string name;
            public Color color;
            public List<Graphic> objects = new List<Graphic>();
        }

        [Header("Controls")]
        [SerializeField] TextMeshProUGUI mainText = null;
        [SerializeField] TextMeshProUGUI footerText = null;
        [SerializeField] TextMeshProUGUI acceptPolicyButtonText = null;
        [SerializeField] TextMeshProUGUI adsConsentToggleText = null;
        [SerializeField] TextMeshProUGUI headerText = null;
        [SerializeField] Button acceptPolicyButton = null;
        [SerializeField] Toggle adsConsentToggle = null;

        [Header( "Fonts" )] 
        [SerializeField] TMP_FontAsset defaultFont;
        [SerializeField] List<TranslationFont> customFonts;
        [SerializeField] string forceLanguage = string.Empty;
        
        [Header( "Color Scheme" )]
        [SerializeField] Color linkColor = Color.blue;
        [SerializeField] List<ColorScheme> elements = new List<ColorScheme>();

        [Header( "Tweening" )]
        public CanvasGroup panel;

        public bool ViewIsValid => mainText != null &&
                                   footerText != null &&
                                   acceptPolicyButton != null &&
                                   adsConsentToggle != null;

        public bool AdsConsentToggle => adsConsentToggle.isOn;
        
        private void Awake()
        {
            if ( ViewIsValid )
            {
                mainText.gameObject.AddComponent<Hyperlink>();
                footerText.gameObject.AddComponent<Hyperlink>();
                Refresh( Debug.isDebugBuild ? forceLanguage : string.Empty );
                acceptPolicyButton.onClick.AddListener( HGenericGDPR.AcceptPolicy );
                panel.alpha = Settings.MIN_ALPHA;
                panel.DOFade(Settings.MAX_ALPHA, Settings.TWEEN_DURATION);
            }
            else
            {
                HLog.LogError( logPrefix, "Some required fields are missing in GDPRView prefab." );
            }
        }

        public void Refresh( string forceLang = "" )
        {
            var translation = GDPRTranslationsProvider.DefaultTranslation;

            if ( HConfigs.HasConfig<GDPRConfig>() )
            {
                var config = HConfigs.GetConfig<GDPRConfig>();
                translation = GDPRTranslationsProvider.GetTranslation( config, forceLang );
                ApplyTranslationFont( translation.lang, config );
            }

            headerText.text = translation.header;
            mainText.text = string.Format( translation.policy, ColorUtility.ToHtmlStringRGB( linkColor ) );
            footerText.text = string.Format( translation.footer, ColorUtility.ToHtmlStringRGB( linkColor ) );
            adsConsentToggleText.text = translation.toggle;
            acceptPolicyButtonText.text = translation.button;

            foreach ( var element in elements )
            {
                foreach ( var graphic in element.objects )
                {
                    graphic.color = element.color;
                }
            }
        }
        
        void ApplyTranslationFont( string country, GDPRConfig config )
        {
            if ( config == null )
            {
                return;
            }
            
            var font = defaultFont;

            var customFont = customFonts.Find( lang => lang.key == country );
            if ( customFont != null && customFont.font != null )
            {
                font = customFont.font;
            }

            mainText.font = font;
            footerText.font = font;
            //acceptPolicyButtonText.font = font;
            adsConsentToggleText.font = font;
            headerText.font = font;
        }
    }
}
