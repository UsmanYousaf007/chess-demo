/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public interface IStoreService
    {
		IPromise<bool> Init(List<string> currencyProductIds);
        IPromise<BackendResult> BuyProduct(string storeProductId);
		string GetItemLocalizedPrice(string productId);
        string GetItemCurrencyCode(string storeProductId);
        decimal GetItemPrice(string storeProductId);
        IPromise<BackendResult> RestorePurchases();
        void UpgardeSubscription(string oldProductId, string newProductId);
    }
}
