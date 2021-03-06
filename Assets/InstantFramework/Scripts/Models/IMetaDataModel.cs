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
        bool ShowChampionshipNewRankDialog { get; set; }

        IAppInfoModel appInfo  { get; set; }
        IStoreSettingsModel store { get; set; }
        IAdsSettingsModel adsSettings { get; set; }
        IRewardsSettingsModel rewardsSettings { get; set; }
        ISettingsModel settingsModel { get; set; }
        IDownloadablesModel downloadableModel { get; set; }
    }
}

