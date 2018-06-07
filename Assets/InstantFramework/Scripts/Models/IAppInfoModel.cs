/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public interface IAppInfoModel
    {
        string appVersion { get; set; }
        string appBackendVersion  { get; set; }

        bool appVersionValid { get; set; }
        string iosURL { get; set; }
        string androidURL { get; set; }

        void Reset();
        void RetrieveAppVersion();
    }
}
