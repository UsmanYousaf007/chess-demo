/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public interface IAppInfoModel
    {
        int appBackendVersion  { get; set; }
        string clientVersion { get; set; }
        bool appBackendVersionValid { get; set; }
        string iosURL { get; set; }
        string androidURL { get; set; }
        int rateAppThreshold { get; set; }
        int onlineCount { get; set; }
        long reconnectTimeStamp { get; set; }
        DisconnectStats isReconnecting { get; set; }
        bool syncInProgress { get; set; }
    }

    public enum DisconnectStats
    {
        FALSE,
        SHORT_DISCONNECT,
        LONG_DISCONNET

    }

}
