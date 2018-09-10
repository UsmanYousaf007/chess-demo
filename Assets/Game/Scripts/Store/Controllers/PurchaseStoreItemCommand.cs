/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
	public class PurchaseStoreItemCommand : Command
	{
		// Command Params
		[Inject] public string key { get; set; }
		[Inject] public bool clearForPurchase { get; set; }

		// Dispatch Signals
        [Inject] public PurchaseStoreItemResultSignal purchaseResultSignal { get; set; }

		// Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }
		[Inject] public IPlayerModel playerModel { get; set; }

		// Services
        [Inject] public IBackendService backendService { get; set; }
		[Inject] public IStoreService storeService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        private StoreItem item;

        public override void Execute()
        {
            item = metaDataModel.store.items[key];
            PurchaseResult purchaseResult = PurchaseResult.NONE;

            if (playerModel.OwnsVGood(key) == true)
            {
                purchaseResult = PurchaseResult.ALREADY_OWNED;
            }
            else if (playerModel.bucks < item.currency2Cost) 
            {
                purchaseResult = PurchaseResult.NOT_ENOUGH_BUCKS;
            } 
            else if (clearForPurchase == false)
            {
                purchaseResult = PurchaseResult.PERMISSION_TO_PURCHASE;
            }

            // Perform purchase if clear for purchase
            if (purchaseResult == PurchaseResult.NONE)
            {
                Purchase(item);
            }
            else
            {
                purchaseResultSignal.Dispatch(item, purchaseResult);
            }
        }

        private void Purchase(StoreItem item)
        {
            if (item.remoteProductId != null) 
            {
                // Purchase from remote store
                storeService.BuyProduct(item.remoteProductId);
            } 
            else 
            {
                // Virtual good purchase
                Retain();
                backendService.BuyVirtualGoods(2, 1, item.key).Then(OnPurchase);
            }
        }

        private void OnPurchase(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                purchaseResultSignal.Dispatch(item, PurchaseResult.PURCHASE_FAILURE);
            }
            else
            {
                purchaseResultSignal.Dispatch(item, PurchaseResult.PURCHASE_SUCCESS);
            }

            Release();
        }
	}
}