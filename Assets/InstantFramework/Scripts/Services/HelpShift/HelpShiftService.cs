using System.Collections.Generic;
using Helpshift;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class HelpShiftService : ISupportService
    {
        //Models
        [Inject] public IPlayerModel playerModel { get; set; }

        private HelpshiftSdk helpshift;
        private Dictionary<string, object> configMap = new Dictionary<string, object>();

        public void Init()
        {
#if !UNITY_EDITOR
            helpshift = HelpshiftSdk.getInstance();
            helpshift.install(Settings.HELPSHIFT_API_KEY, Settings.HELPSHIFT_DOMAIN_NAME, Settings.HELPSHIFT_APP_ID, configMap);
#endif
        }

        public void ShowConversation()
        {
#if !UNITY_EDITOR
            helpshift.showConversation(configMap);
#endif
        }

        public void ShowFAQ()
        {
#if !UNITY_EDITOR
            var customMetaDeta = new Dictionary<string, object>();
            SetupConfigMap();
            customMetaDeta.Add(HelpshiftSdk.HSCUSTOMMETADATAKEY, configMap);
            helpshift.showFAQs(customMetaDeta);
#endif
        }

        private void SetupConfigMap()
        {
            configMap.Add("DisplayName", playerModel.name);
            configMap.Add("EditedName", playerModel.editedName);
            configMap.Add("PlayerTag", playerModel.tag);
            configMap.Add("IsPremium", playerModel.isPremium);
        }
    }
}
