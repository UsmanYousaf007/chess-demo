/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface IStoreSettingsModel
    {
        void Reset();
        void Initialize();
        void Add(string kind, IOrderedDictionary<string, ShopItem> kindItems);
        List<string> getProductIds();

        IDictionary<string, List<ShopItem>> lists { get; set; }
        IOrderedDictionary<string, ShopItem> items { get; set; }
    }
}

