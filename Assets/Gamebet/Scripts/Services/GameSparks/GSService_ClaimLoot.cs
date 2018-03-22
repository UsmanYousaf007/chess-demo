
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System.Collections.Generic;
using GameSparks.Core;
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        public IPromise<BackendResult> ClaimLoot(string key)
        {
            return new GSClaimLootRequest().Send(key, OnClaimLootSuccess);
        }

        private void OnClaimLootSuccess(LogEventResponse response)
        {
            if (response.HasErrors)
            {
                backendErrorSignal.Dispatch(BackendResult.CLAIM_LOOT_FAILED);
            }
            else
            {
                if (response.ScriptData != null)
                {
                    // Process granted loot
                    if (response.ScriptData.ContainsKey(GSBackendKeys.LOOT_BOXES_PLAYER_LOOT_OPENED))
                    {
                        IList<GSData> lootBoxesData = response.ScriptData.GetGSDataList(GSBackendKeys.LOOT_BOXES_PLAYER_LOOT_OPENED);
                        IList<LootBox> lootBoxItems = new List<LootBox>();
                        GSParser.PopulateLootBoxes(lootBoxItems, lootBoxesData);

                        // Expect a single lootbox

                        // Grant any coins from the loot
                        playersModel.currency1 += lootBoxItems[0].coins;

                        // Grant shop items from the loot
                        foreach (LootShopItem lootShopItem in lootBoxItems[0].shopItems)
                        {
                            if (inventoryModel.allShopItems.ContainsKey(lootShopItem.shopItemKey))
                            {
                                inventoryModel.allShopItems[lootShopItem.shopItemKey] += lootShopItem.quantity;
                            }
                            else
                            {
                                inventoryModel.allShopItems.Add(lootShopItem.shopItemKey, lootShopItem.quantity);
                            }
                        }

                    }

                    if (response.ScriptData.ContainsKey(GSBackendKeys.LOOT_BOXES_PLAYER_LOOT))
                    {
                        IList<GSData> lootBoxesData = response.ScriptData.GetGSDataList(GSBackendKeys.LOOT_BOXES_PLAYER_LOOT);
                        IList<LootBox> lootBoxItems = new List<LootBox>();
                        GSParser.PopulateLootBoxes(lootBoxItems, lootBoxesData);

                        LogUtil.Log("I have opened the box ", "red");

                        inventoryModel.lootBoxItems.Clear();
                        inventoryModel.lootBoxItems = lootBoxItems;
                    }
                    else
                    {
                        inventoryModel.lootBoxItems.Clear();
                    }
                }
            }
        }
    }
}

