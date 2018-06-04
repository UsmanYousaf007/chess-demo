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
        public IStoreSettingsModel store { get; set; }

        public AdSettings adSettings { get; set; }
        public int defaultStartingBucks { get; set; }
        public string[] defaultVGoods { get; set; }

        [PostConstruct]
		public void Load()
		{
            Reset();
		}

        public void AddAdSettings(AdSettings settings)
        {
            adSettings = settings;
        }

        private void Reset()
        {
            adSettings = null;
        }
    }

    #region DataStructures

    public class AdSettings
    {
        public int maxImpressionsPerSlot;
        public int slotMinutes;
        public int adsRewardIncrement;
    }

    #endregion
}
