using HUF.Utils.Configs.API;
using HUFEXT.GenericGDPR.Runtime.API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HUFEXT.GenericGDPR.Runtime.Implementation
{
    public class GenericGDPRView : MonoBehaviour
    {
        [Header("Content")]
        [SerializeField] TextMeshProUGUI m_headerText;
        [SerializeField] TextMeshProUGUI m_contentText;
        [SerializeField] TextMeshProUGUI m_moreInfoText;
        [SerializeField] TextMeshProUGUI m_persnalizedAdsText;
        [SerializeField] Toggle m_personalizedAdsToggle;

        [Header("Button")]
        [SerializeField] Button m_button;
        [SerializeField] TextMeshProUGUI m_buttonText;

        GDPRConfig m_config;
        bool m_detailsEnabled = false;
        bool m_adsConsent = true;

        public bool AdsConsent => m_adsConsent;

        public void Init()
        {
            if( HConfigs.HasConfig<GDPRConfig>() )
            {
                m_config = HConfigs.GetConfig<GDPRConfig>();
                m_headerText.text = m_config.titleText;
                m_buttonText.text = m_config.buttonText;
                m_persnalizedAdsText.text = m_config.personalizedAdsText;

                m_personalizedAdsToggle.onValueChanged.AddListener( OnToggle );
                m_button.onClick.AddListener( HGenericGDPR.AcceptPolicy );
            }
            else
            {
                Debug.Log( "[GenericGDPR] Unable to initialize component. Missing config." );
            }
        }

        public void ToggleDetails()
        {
            if( m_detailsEnabled )
            {
                m_contentText.text = m_config.policyText;
                m_moreInfoText.text = m_config.moreInfoText;
                m_detailsEnabled = false;
            }
            else
            {
                m_contentText.text = m_config.detailedPolicyText;
                m_moreInfoText.text = m_config.lessInfoText;
                m_detailsEnabled = true;
            }
        }

        void OnToggle( bool consent )
        {
            m_adsConsent = consent;
        }

        void OnDisable()
        {
            m_personalizedAdsToggle.onValueChanged.RemoveListener( OnToggle );
            m_button?.onClick.RemoveListener( HGenericGDPR.AcceptPolicy );
        }
    }
}