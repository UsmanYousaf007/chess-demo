using System;
using HUF.Utils.Configs.API;
using UnityEngine;

namespace HUFEXT.GenericGDPR.Runtime.API
{
    [Serializable]
    public enum LinkType
    {
        PrivacyPolicy,
        TermsOfUse,
        EULA
    }
    
    [CreateAssetMenu(menuName = "HUFEXT/GDPR/GDPR Config", fileName = "GDPRConfig.asset")]
    public class GDPRConfig : AbstractConfig
    {
        [Serializable]
        public class Link
        {
            public LinkType Type;
            public string Text;
            public string URL;
        }

        [Header( "Common" )] 
        [SerializeField] GameObject prefab;

        [SerializeField] bool autoInit = true;
        [SerializeField] bool destroyOnAccept = true;
        [SerializeField] bool usePlayerPrefs = true;
        [SerializeField] string playerPrefsKey;
        
        [Header("Texts")]
        public string titleText;
        public string policyText;
        public string detailedPolicyText;
        public string moreInfoText;
        public string lessInfoText;
        public string buttonText;

        [Header( "Links" )] 
        [SerializeField] Link privacyPolicy;
        [SerializeField] Link termsOfUse;
        [SerializeField] Link eula;

        public GameObject Prefab => prefab;
        public bool AutoInit => autoInit;
        public bool DestroyOnAccept => destroyOnAccept;
        public bool UsePlayerPrefs => usePlayerPrefs;
        public string PlayerPrefsKey => playerPrefsKey;
        public Link PrivacyPolicy => privacyPolicy;
        public Link TermsOfUse => termsOfUse;
        public Link Eula => eula;
    }
}