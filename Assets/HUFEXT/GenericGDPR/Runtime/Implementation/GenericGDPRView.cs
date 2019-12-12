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

        [Header("Button")]
        [SerializeField] Button m_button;
        [SerializeField] TextMeshProUGUI m_buttonText;

        GDPRConfig m_config;
        bool m_detailsEnabled = false;

        public void Init()
        {
            if( HConfigs.HasConfig<GDPRConfig>() )
            {
                m_config = HConfigs.GetConfig<GDPRConfig>();
                m_headerText.text = m_config.titleText;
                m_buttonText.text = m_config.buttonText;

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

        void OnDisable()
        {
            m_button?.onClick.RemoveListener( HGenericGDPR.AcceptPolicy );
        }
    }
}