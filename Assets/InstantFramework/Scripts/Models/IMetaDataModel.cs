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
		bool remoteStoreAvailable { get; set; }
		IOrderedDictionary<string, StoreItem> items { get; set; }		// All items dictionary
        IDictionary<string, List<StoreItem>> lists { get; set; }		// Categorized lists of items by kind
        AdSettings adSettings { get; set; }                             // Ad settings

		void AddStoreItem(string kind, IOrderedDictionary<string, StoreItem> kindItems);
		List<string> getRemoteProductIds();

        void AddAdSettings(AdSettings settings);

	}
}

