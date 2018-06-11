/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class MetaDataModel : IMetaDataModel
    {
        public IAppInfoModel appInfo  { get; set; }
        public IStoreSettingsModel store { get; set; }
        public IAdsSettingsModel adsSettings { get; set; }

        public void Reset()
        {
            appInfo = null;
            store = null;
            adsSettings = null;
        }
    }
}
