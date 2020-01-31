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
        bool remoteStoreAvailable { get; set; }
        IDictionary<string, List<StoreItem>> lists { get; set; }
        IOrderedDictionary<string, StoreItem> items { get; set; }
        void Add(string kind, IOrderedDictionary<string, StoreItem> kindItems);
        List<string> getRemoteProductIds();
        StoreItem GetItemBySkinIndex(int skinIndex);
    }
}

