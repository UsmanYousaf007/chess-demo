/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface IStoreService
    {
		void Init(List<string> currencyProductIds);
		void BuyProduct (string storeProductId);
		string GetItemLocalizedPrice (string productId);
    }
}
