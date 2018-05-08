/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public interface IStoreSettingsModel
    {
		IOrderedDictionary<string, StoreItem> items { get; set; }		// All items dictionary
        IDictionary<string, List<StoreItem>> lists { get; set; }		// Categorized lists of items by kind

		void Load();
		void Add(string kind, IOrderedDictionary<string, StoreItem> kindItems);
		List<string> getRemoteProductIds();
	}
}

