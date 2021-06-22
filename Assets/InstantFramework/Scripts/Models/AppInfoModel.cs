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
        public bool isAutoSubscriptionDlgShown { get; set; }
        public bool isResumeGS { get; set; }
        public bool isVideoLoading { get; set; }
        public bool isMandatoryUpdate { get; set; }
        public string storeURL { get; set; }
        public int nthWinsRateApp { get; set; }
        public int gamesPlayedCount { get; set; }
        public bool showGameUpdateBanner { get; set; }

        public string privacyPolicyURL
        {
            get
            {
                return "https://huuugegames.com/privacy-policy/";
            }
        }

        public string adPartnersURL
        {
            get
            {
                return "\"https://huuugegames.com/privacy-policy/#huuuge%27s+partner+list\"";
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

        public string chatOnDiscordURL
        {
            get
            {
                return "https://discord.gg/QUuudB4";
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
            isResumeGS = false;

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
            isAutoSubscriptionDlgShown = false;
            isVideoLoading = false;
            isMandatoryUpdate = false;
            storeURL = "";
            nthWinsRateApp = 10;
            gamesPlayedCount = 0;
            showGameUpdateBanner = false;
        }
    }
}
