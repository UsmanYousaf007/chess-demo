/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> VirtualGoodsTransaction(GSRequestData jsonData)
        {
            return new GSVirtualGoodsTransaction(GetRequestContext()).Send(jsonData, OnVirtualGoodsTransactionSuccess, OnVirtualGoodsTransactionFailed);
        }

        public class GSVirtualGoodsTransaction : GSFrameworkRequest
        {
            const string SHORT_CODE = "VirtualGoodsTransaction";
            const string ATT_JSON_DATA = "jsonData";

            public GSVirtualGoodsTransaction(GSFrameworkRequestContext context) : base(context) { }

            public IPromise<BackendResult> Send(GSRequestData jsonData, Action<object, Action<object>> onSuccess, Action<object> onFailure)
            {
                this.onSuccess = onSuccess;
                this.onFailure = onFailure;
                this.errorCode = BackendResult.VIRTUAL_GOODS_TRANSACTION_FAILED;

                new LogEventRequest()
                    .SetEventKey(SHORT_CODE)
                    .SetEventAttribute(ATT_JSON_DATA, jsonData)
                    .Send(OnRequestSuccess, OnRequestFailure);

                return promise;
            }
        }

        private void OnVirtualGoodsTransactionSuccess(object r, Action<object> a)
        {
            LogEventResponse response = (LogEventResponse)r;

            if (response != null && response.ScriptData != null)
            {
                var boughtShortCode = response.ScriptData.GetString("buyShortCode");
                var consumedShortCode = response.ScriptData.GetString("consumeShortCode");
                var boughtQuantity = GSParser.GetSafeInt(response.ScriptData, "buyQuantity");
                var consumedQuantity = GSParser.GetSafeInt(response.ScriptData, "consumeQuantity");

                if (!string.IsNullOrEmpty(boughtShortCode))
                {
                    if (boughtShortCode.Equals(GSBackendKeys.PlayerDetails.GEMS))
                    {
                        playerModel.gems += boughtQuantity;
                    }
                    else if (boughtShortCode.Equals(GSBackendKeys.PlayerDetails.COINS))
                    {
                        playerModel.coins += boughtQuantity;
                    }
                    else if (playerModel.inventory.ContainsKey(boughtShortCode))
                    {
                        playerModel.inventory[boughtShortCode] += boughtQuantity;
                    }
                    else
                    {
                        playerModel.inventory.Add(boughtShortCode, boughtQuantity);
                    }
                }

                if (!string.IsNullOrEmpty(consumedShortCode))
                {
                    if (consumedShortCode.Equals(GSBackendKeys.PlayerDetails.GEMS))
                    {
                        playerModel.gems -= consumedQuantity;
                    }
                    else if (consumedShortCode.Equals(GSBackendKeys.PlayerDetails.COINS))
                    {
                        playerModel.coins -= consumedQuantity;
                    }
                    else if (playerModel.inventory.ContainsKey(consumedShortCode))
                    {
                        playerModel.inventory[consumedShortCode] -= consumedQuantity;
                    }

                    var challengeId = response.ScriptData.GetString("challengeId");

                    if (!string.IsNullOrEmpty(challengeId) && matchInfoModel.matches.ContainsKey(challengeId))
                    {
                        matchInfoModel.matches[challengeId].playerPowerupUsedCount++;

                        if (matchInfoModel.matches[challengeId].freeHints > 0)
                        {
                            matchInfoModel.matches[challengeId].freeHints--;
                        }
                    }
                }
            }
        }

        private void OnVirtualGoodsTransactionFailed(object r)
        {
            var response = (LogEventResponse)r;
            var errorData = response.Errors;
            var errorString = errorData.GetString("error");

            if (errorString.Equals("coinsInsufficient"))
            {
                playerModel.coins = GSParser.GetSafeInt(errorData, "coins");
                updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            }
            else if (errorString.Equals("gemsInsufficient"))
            {
                playerModel.gems = GSParser.GetSafeInt(errorData, "gems");
                updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            }
        }
    }
}
