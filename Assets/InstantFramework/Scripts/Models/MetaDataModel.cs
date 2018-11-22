/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    // This is a convenience grouping model.
    // Does not have a lifecycle (reset/load etc)
    public class MetaDataModel : IMetaDataModel
    {
        public IAppInfoModel appInfo { get; set; } = null;
        public IStoreSettingsModel store { get; set; } = null;
        public IAdsSettingsModel adsSettings { get; set; } = null;
    }
}
