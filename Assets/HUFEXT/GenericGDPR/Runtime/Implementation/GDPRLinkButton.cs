using HUF.Utils.Configs.API;
using HUFEXT.GenericGDPR.Runtime.API;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HUFEXT.GenericGDPR.Runtime.Implementation
{
    public class GDPRLinkButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] LinkType linkType;
        [SerializeField] TextMeshProUGUI linkText;
        string linkUrl;
        
        protected void OnEnable()
        {
            Initialize();
        }

        void Initialize()
        {
            var config = HConfigs.GetConfig<GDPRConfig>();
            if( config == null || linkText == null )
            {
                return;
            }
            
            switch( linkType )
            {
                case LinkType.PrivacyPolicy:
                {
                    linkText.text = config.PrivacyPolicy.Text;
                    linkUrl = config.PrivacyPolicy.URL;
                    break;
                }
                case LinkType.TermsOfUse:
                {
                    linkText.text = config.TermsOfUse.Text;
                    linkUrl = config.TermsOfUse.URL;
                    break;
                }
                case LinkType.EULA:
                {
                    linkText.text = config.Eula.Text;
                    linkUrl = config.Eula.URL;
                    break;
                }
            }
        }

        public void OnPointerDown( PointerEventData eventData )
        {
            Application.OpenURL( linkUrl );
        }
    }
}
