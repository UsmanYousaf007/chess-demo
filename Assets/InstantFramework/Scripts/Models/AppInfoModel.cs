/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class AppInfoModel : IAppInfoModel
    {
        public int appBackendVersion  { get; set; }
        public string clientVersion { get; set; }
        public bool appBackendVersionValid { get; set; }
        public string iosURL { get; set; }
        public string androidURL { get; set; }
        public int rateAppThreshold { get; set; }
        public int onlineCount { get; set; }
        public string contactSupportURL { get; set; }
        //Persistant field ---- do not reset  
        public long reconnectTimeStamp { get; set; }
        public DisconnectStates isReconnecting { get; set; }
        public bool syncInProgress { get; set; }
        public GameMode gameMode { get; set; }
        public bool isNotificationActive { get; set; }
        public InternalAdType internalAdType { get; set; }

        public string privacyPolicyURL
        {
            get
            {
                return "https://huuugegames.com/privacy-policy/";
            }
        }

        public string faqURL
        {
            get
            {
                return "https://huuuge.helpshift.com/a/chess-stars/";
            }
        }

        public string termsOfUseURL
        {
            get
            {
                return "https://huuugegames.com/terms-of-use";
            }
        }

        enum subVersionIndex
        {
            MAJOR = 0,
            MINOR = 1,
            BACKEND = 2
        }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            string[] version = Application.version.Split('.');
            appBackendVersion = int.Parse(version[(int)subVersionIndex.BACKEND]);
            clientVersion = Application.version;
            gameMode = GameMode.NONE;
            isNotificationActive = false;

            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            appBackendVersionValid = false;
            iosURL = "";
            androidURL = "";
            rateAppThreshold = 0;
            onlineCount = 0;
            isNotificationActive = false;
            contactSupportURL = "";
            internalAdType = InternalAdType.NONE;
        }
    }
}
