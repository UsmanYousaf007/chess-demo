/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public interface IMetaDataModel
    {
        IStoreSettingsModel store { get; set; }
        AdSettings adSettings { get; set; }

        int defaultStartingBucks { get; set; }
        string[] defaultVGoods { get; set; }

        void AddAdSettings(AdSettings settings);

	}
}

