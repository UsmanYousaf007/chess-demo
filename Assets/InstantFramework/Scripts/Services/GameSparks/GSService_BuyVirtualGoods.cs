

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System.Collections.Generic;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public IPlayerModel playersModel { get; set; }

        public IPromise<BackendResult> BuyVirtualGoods(int currencyType, int quantity, string shortCode)
        {
            return new GSBuyVirtualGoodsRequest().Send(currencyType, quantity, shortCode, OnBuyVirtualGoodsSuccess);
        }

        private void OnBuyVirtualGoodsSuccess(BuyVirtualGoodResponse response)
        {
            if (response.HasErrors)
            {
                backendErrorSignal.Dispatch(BackendResult.BUY_VIRTUAL_GOOD_FAILED);
            }
            else
            {

                if (response.ScriptData != null)
                {
                    // Bought coins
                    if (response.ScriptData.ContainsKey(GSBackendKeys.COINSPACK_COINS1_BOUGHT))
                    {
                        int coins1Added = response.ScriptData.GetInt(GSBackendKeys.COINSPACK_COINS1_BOUGHT).Value;
                        playersModel.currency1 += coins1Added;
                    }
                }

                // Consume bucks
                if (response.CurrencyType == 2 && response.CurrencyConsumed.HasValue)
                {
                    playersModel.currency2 -= response.CurrencyConsumed.Value;
                }

                GSEnumerable<BuyVirtualGoodResponse._Boughtitem> virtualGoods = response.BoughtItems;
                foreach (var v in virtualGoods)
                {
                    //long quantity = v.Quantity.Value;
                    string shopItemId = v.ShortCode;

                    int count = 0;
                    if (inventoryModel.items.ContainsKey(shopItemId))
                    {
                        count = inventoryModel.items[shopItemId] + 1;
                        inventoryModel.items[shopItemId] = count;
                    }
                    else
                    {
                        inventoryModel.items.Add(shopItemId, 1); 
                    }
                }
            }
        }
    }
}
