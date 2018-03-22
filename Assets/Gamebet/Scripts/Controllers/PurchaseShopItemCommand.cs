
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace TurboLabz.Gamebet
{
    public class PurchaseShopItemCommand : Command
    {
        // Models
        [Inject] public IInventoryModel inventoryModel { get; set; }
        [Inject] public IPlayerModel playersModel { get; set; }
        [Inject] public IShopSettingsModel shopSettingsModel { get; set; }
        // VO
        [Inject] public Signal purchaseResultSignal { get; set; }
        [Inject] public string activeShopItemId { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        public override void Execute()
        {
            Retain();
            ProcessPurchaseShopItem(activeShopItemId);
        }

        public void ProcessPurchaseShopItem(string shopItemId)
        {
            ShopItem shopItem = shopSettingsModel.allShopItems[shopItemId];
            TurboLabz.Common.LogUtil.Log("******************************PurchaseShopItemCommand KIND=" + shopItem.kind);

            // Buy coin pack with bucks
            if (shopItem.kind == GSBackendKeys.ShopItem.COINS_SHOP_TAG)
            {
                if (shopItem.type == GSBackendKeys.ShopItem.SHOP_ITEM_TYPE_CURRENCY)
                {
                    // Buying bucks
                    InAppPurchase.instance.BuyProduct(shopItem.id);
                }
                else
                {
                    // Buying coins
                    backendService.BuyVirtualGoods(2, 1, shopItem.id).Then(OnProcessPurchaseShopItem);
                }
            }
            else
            {
                // Buy non-currency shop item
                backendService.BuyVirtualGoods(2, 1, shopItem.id).Then(OnProcessPurchaseShopItem);
            }
        }

        private void OnProcessPurchaseShopItem(BackendResult result)
        {
            TurboLabz.Common.LogUtil.Log("******************************PurchaseShopItemCommand RESULT" + result);

            if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }
            purchaseResultSignal.Dispatch();
            Release();
        }
    }
}
