/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public interface IAppInfoModel
    {
        int appBackendVersion  { get; set; }
        bool appBackendVersionValid { get; set; }
        string iosURL { get; set; }
        string androidURL { get; set; }
        int rateAppThreshold { get; set; }
    }
}
