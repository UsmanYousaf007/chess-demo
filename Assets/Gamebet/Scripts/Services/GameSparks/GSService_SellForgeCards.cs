
using GameSparks.Api.Responses;
using strange.extensions.promise.api;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        public IPromise<BackendResult> SellForgeCards(string consumeVGoodShortCode,
            int consumeVGoodQuantity)
        {
            return new GSSellForgeCardsRequest().Send(consumeVGoodShortCode, 
                consumeVGoodQuantity, 
                OnSellForgeCardsGoodSuccess);
        }

        private void OnSellForgeCardsGoodSuccess(LogEventResponse response)
        {
            if (response.HasErrors)
            {
                backendErrorSignal.Dispatch(BackendResult.SELL_FORGE_CARDS_FAILED);
            }
            else
            {
                if (response.ScriptData != null)
                {
                    TurboLabz.Common.LogUtil.Log("********* OnSellForgeCardsGoodSuccess Data---> " + response.ScriptData.JSON);

                    // Gained coins
                    if (response.ScriptData.ContainsKey(GSBackendKeys.FORGE_CARDS_COINS_GAINED))
                    {
                        int coins1Added = response.ScriptData.GetInt(GSBackendKeys.FORGE_CARDS_COINS_GAINED).Value;
                        playersModel.currency1 += coins1Added;

                        string forgeCardKey = response.ScriptData.GetString(GSBackendKeys.FORGE_ITEM_CARD_KEY);
                        int quantity = response.ScriptData.GetInt(GSBackendKeys.FORGE_CARDS_SOLD_QUANTITY).Value;

                        int count = inventoryModel.allShopItems[forgeCardKey];
                        count = count - quantity;

                        if (count <= 0)
                        {
                            inventoryModel.allShopItems.Remove(forgeCardKey);
                        }
                        else
                        {
                            inventoryModel.allShopItems[forgeCardKey] -= quantity;
                        }

                        TurboLabz.Common.LogUtil.Log("foregecardKey: " + forgeCardKey + " quantity: " + quantity + " count: " + count + " result: " + inventoryModel.allShopItems[forgeCardKey] , "red");

                        TurboLabz.Common.LogUtil.Log("********* OnSellForgeCardsGoodSuccess COINS ADDED ---> " + coins1Added);
                    }
                }
            }
        }
    }
}

