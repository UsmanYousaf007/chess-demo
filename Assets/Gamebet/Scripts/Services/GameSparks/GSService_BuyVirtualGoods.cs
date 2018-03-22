

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System.Collections.Generic;
using GameSparks.Core;

namespace TurboLabz.Gamebet
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
                    TurboLabz.Common.LogUtil.Log("********* BuyVirtualGoods Data---> " + response.ScriptData.JSON);

                    // Process loot data
                    if (response.ScriptData.ContainsKey(GSBackendKeys.LOOT_BOXES_PLAYER_LOOT))
                    {
                        IList<GSData> lootBoxesData = response.ScriptData.GetGSDataList(GSBackendKeys.LOOT_BOXES_PLAYER_LOOT);
                        if (lootBoxesData != null && lootBoxesData.Count > 0)
                        {
                            inventoryModel.lootBoxItems.Clear();
                            GSParser.PopulateLootBoxes(inventoryModel.lootBoxItems, lootBoxesData);
                        }
                    }

                    // Bought coins
                    if (response.ScriptData.ContainsKey(GSBackendKeys.COINSPACK_COINS1_BOUGHT))
                    {
                        int coins1Added = response.ScriptData.GetInt(GSBackendKeys.COINSPACK_COINS1_BOUGHT).Value;
                        playersModel.currency1 += coins1Added;
                        TurboLabz.Common.LogUtil.Log("********* BuyVirtualGoods COINS ADDED ---> " + coins1Added);
                    }
                }

                // Bought bucks?
                //if (response.Currency2Added.HasValue)
                //{
                //    playersModel.currency2 += response.Currency2Added.Value;
                //    TurboLabz.Common.LogUtil.Log("********* BuyVirtualGoods COINS ADDED ---> " + response.Currency2Added.Value);
                //}

                // Consume bucks
                if (response.CurrencyType == 2 && response.CurrencyConsumed.HasValue)
                {
                    playersModel.currency2 -= response.CurrencyConsumed.Value;
                    TurboLabz.Common.LogUtil.Log("********* BuyVirtualGoods BUCKS CONSUMED ---> " + response.CurrencyConsumed.Value);
                }

                GSEnumerable<BuyVirtualGoodResponse._Boughtitem> virtualGoods = response.BoughtItems;
                foreach (var v in virtualGoods)
                {
                    //long quantity = v.Quantity.Value;
                    string shopItemId = v.ShortCode;

                    int count = 0;
                    if (inventoryModel.allShopItems.ContainsKey(shopItemId))
                    {
                        count = inventoryModel.allShopItems[shopItemId] + 1;
                        inventoryModel.allShopItems[shopItemId] = count;
                    }
                    else
                    {
                        inventoryModel.allShopItems.Add(shopItemId, 1); 
                    }

                    TurboLabz.Common.LogUtil.Log("********* BuyVirtualGoods ITEM PURCHASED ---> " + shopItemId);
                }
            }
        }
    }
}
