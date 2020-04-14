/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

//using HUF.Analytics.API;
using strange.extensions.command.impl;
using strange.extensions.promise.api;
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
        [Inject] public UpdatePlayerInventorySignal updatePlayerInventorySignal { get; set; }

        // Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }
		[Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
		[Inject] public IStoreService storeService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        private StoreItem item;
        private NS pState = null;

        public override void Execute()
        {
            item = metaDataModel.store.items[key];
            pState = navigatorModel.previousState;

            PurchaseResult purchaseResult = PurchaseResult.NONE;

            if (item.maxQuantity == 1 && playerModel.OwnsVGood(key) == true)
            {
                purchaseResult = PurchaseResult.ALREADY_OWNED;
            }
            else if (item.currency2Cost != 0 && playerModel.bucks < item.currency2Cost) 
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

        private void Purchase(StoreItem storeItem)
        {
            if (storeItem.remoteProductId != null) 
            {
                OnPromiseReturn(BackendResult.PURCHASE_ATTEMPT);

                // Purchase from remote store
                IPromise<BackendResult> promise = storeService.BuyProduct(storeItem.remoteProductId);
                if (promise != null)
                {
                    promise.Then(OnPromiseReturn);
                }
            } 
            else 
            {
                // Virtual good purchase
                Retain();
                backendService.BuyVirtualGoods(2, 1, storeItem.key).Then(OnPurchase);
            }
        }

        private void OnPromiseReturn(BackendResult result)
        {
            string eventName = "attempt";
            string cameFromScreen = "settings";

            if (pState.GetType() == typeof(NSLobby))
            {
                cameFromScreen = "lobby_banner";

                if (metaDataModel.appInfo.isAutoSubscriptionDlgShown)
                {
                    cameFromScreen = "auto_popup";
                }
            }
            else if (pState.GetType() == typeof(NSCPU) || pState.GetType() == typeof(NSMultiplayer))
            {
                cameFromScreen = "power_ups";
            }
            else if (pState.GetType() == typeof(NSThemeSelectionDlg))
            {
                cameFromScreen = "theme_selection";
            }
            else if (pState.GetType() == typeof(NSCPUResultsDlg) || pState.GetType() == typeof(NSMultiplayerResultsDlg))
            {
                if (metaDataModel.appInfo.internalAdType == InternalAdType.FORCED_ON_WIN)
                {
                    cameFromScreen = "victory_popup";
                }
                else if (metaDataModel.appInfo.internalAdType == InternalAdType.INTERAL_AD)
                {
                    cameFromScreen = "interal_ad";
                }
            }

            if (result == BackendResult.PURCHASE_ATTEMPT)
            {
                eventName = "attempt";
            }
            else if (result == BackendResult.PURCHASE_COMPLETE)
            {
                eventName = "completed";
            }
            else if (result == BackendResult.PURCHASE_CANCEL)
            {
                eventName = "cancelled";
            }
            else if (result == BackendResult.PURCHASE_FAILED)
            {
                eventName = "failed";
            }

            hAnalyticsService.LogMonetizationEvent(eventName, item.currency1Cost, "iap_purchase", $"subscription_{item.displayName.Replace(" ","_")}", cameFromScreen);
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
                updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
                audioService.Play(audioService.sounds.SFX_SHOP_PURCHASE_ITEM);
            }

            Release();
        }
	}
}