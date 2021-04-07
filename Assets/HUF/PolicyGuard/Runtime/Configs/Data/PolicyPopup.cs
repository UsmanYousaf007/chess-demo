using HUF.GenericDialog.Runtime.Configs;

namespace HUF.PolicyGuard.Runtime.Configs.Data
{
    [System.Serializable]
    public class PolicyPopup
    {
        [System.Serializable]
        public enum PopupType
        {
            GDPR = 0,
            GDPRWithAds = 1,
            ATT = 2,
            PersonalizedAds = 3,
            Custom = 99
        }

        public string name;
        public PopupType type;
        public string playerPrefsKey;
        public HGenericDialogConfig popupConfig;
        public bool setKeyAutomatically;

        public PolicyPopup()
        {
            this.type = PopupType.GDPR;
        }

        public PolicyPopup( PopupType type )
        {
            this.type = type;
        }
    }
}