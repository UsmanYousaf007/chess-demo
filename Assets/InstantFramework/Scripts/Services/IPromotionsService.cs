/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface IPromotionsService 
    {
        List<List<string>> promotionsSequence { get; set; }
        bool promotionShown { get; }
        void LoadPromotion();
        void LoadRemoveAdsPromotion();
        void LoadSubscriptionPromotion();
        bool IsSaleActive(string key);
        bool isDynamicBundleShownOnLaunch { get; }
    }
}
