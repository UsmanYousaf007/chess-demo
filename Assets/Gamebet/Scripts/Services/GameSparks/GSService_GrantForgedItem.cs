
using GameSparks.Api.Responses;
using strange.extensions.promise.api;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        public IPromise<BackendResult> GrantForgedItem(string consumeVGoodShortCode)
        {
            return new GSGrantForgedItemRequest().Send(consumeVGoodShortCode, 
                                                              OnGrantForgedItemSuccess);
        }

        private void OnGrantForgedItemSuccess(LogEventResponse response)
        {
            if (response.HasErrors)
            {
                backendErrorSignal.Dispatch(BackendResult.GRANT_FORGED_VIRTUAL_GOOD_FAILED);
            }
            else
            {
                if (response.ScriptData != null)
                {
                    TurboLabz.Common.LogUtil.Log("********* OnGrantForgedItemSuccess Data---> " + response.ScriptData.JSON);

                    // Forged item granted
                    if (response.ScriptData.ContainsKey(GSBackendKeys.FORGE_CARDS_FORGE_ITEM_GRANTED))
                    {
                        string forgedItemGrantedId = response.ScriptData.GetString(GSBackendKeys.FORGE_CARDS_FORGE_ITEM_GRANTED);
                        inventoryModel.allShopItems.Add(forgedItemGrantedId, 1); 

                        TurboLabz.Common.LogUtil.Log("********* OnGrantForgedItemSuccess ITEM ADDED ---> " + forgedItemGrantedId);
                    }

                    if (response.ScriptData.ContainsKey(GSBackendKeys.FORGE_ITEM_CARD_KEY))
                    {
                        string forgedCardKey = response.ScriptData.GetString(GSBackendKeys.FORGE_ITEM_CARD_KEY);
                        int forgedCardConsumedQuantity = response.ScriptData.GetInt(GSBackendKeys.FORGE_CARDS_CONSUMED_QUANTITY).Value;

                        int count = inventoryModel.allShopItems[forgedCardKey] - forgedCardConsumedQuantity;

                        if (count == 0)
                        {
                            inventoryModel.allShopItems.Remove(forgedCardKey);
                        }
                        else
                        {
                            inventoryModel.allShopItems[forgedCardKey] -= forgedCardConsumedQuantity;
                        } 

                        TurboLabz.Common.LogUtil.Log("********* Remaining forge cards ---> " + inventoryModel.allShopItems[forgedCardKey] + " count " + count , "red");
                        TurboLabz.Common.LogUtil.Log("********* OnGrantForgedItemSuccess FORGE ITEM KEY ---> " + forgedCardKey);
                        TurboLabz.Common.LogUtil.Log("********* OnGrantForgedItemSuccess FORGE ITEM CONSUMED QUANTITY ---> " + forgedCardConsumedQuantity);
                    }
                }
            }
        }
    }
}
