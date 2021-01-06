/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using GameSparks.Core;
using TurboLabz.TLUtils;
using TurboLabz.CPU;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public CancelHintSingal cancelHintSingal { get; set; }
        [Inject] public RatingBoostedSignal ratingBoostedSignal { get; set; }

        public IPromise<BackendResult> ClaimReward(GSRequestData jsonData)
        {
            return new GSClaimRewardRequest(GetRequestContext()).Send(jsonData, OnClaimRewardSuccess);
        }

        private void OnClaimRewardSuccess(object r, Action<object> a)
        {
            LogEventResponse response = (LogEventResponse)r;
            if (response != null && response.ScriptData != null)
            {
                var data = response.ScriptData;
                var rewardType = GSParser.GetSafeString(data, GSBackendKeys.ClaimReward.CLAIM_REWARD_TYPE);

                switch (rewardType)
                {
                    case GSBackendKeys.ClaimReward.TYPE_BOOST_RATING:
                        var vo = new EloVO();
                        var eloChange = GSParser.GetSafeInt(data, GSBackendKeys.Rewards.RATING_BOOST);
                        playerModel.eloScore += eloChange;
                        vo.playerEloScore = playerModel.eloScore;
                        updateEloScoresSignal.Dispatch(vo);
                        ratingBoostedSignal.Dispatch(eloChange);
                        playerModel.gems -= GSParser.GetSafeInt(data, GSBackendKeys.PlayerDetails.GEMS);
                        analyticsService.Event(AnalyticsEventId.booster_used, AnalyticsContext.rating_booster);
                        break;

                    case GSBackendKeys.ClaimReward.TYPE_COINS_PURCHASE:
                    case GSBackendKeys.ClaimReward.TYPE_DAILY:
                        ParseRewards(data.GetGSData(GSBackendKeys.ClaimReward.REWARD_INFO));
                        break;

                    case GSBackendKeys.ClaimReward.TYPE_LOBBY_CHEST:
                        ParseRewards(data.GetGSData(GSBackendKeys.ClaimReward.REWARD_INFO));
                        playerModel.chestUnlockTimestamp = GSParser.GetSafeLong(data, GSBackendKeys.PlayerDetails.CHEST_UNLOCK_TIMESTAMP);
                        break;
                }

                updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            }
        }

        private void ParseRewards(GSData data)
        {
            if (data != null)
            {
                foreach (var reward in data.BaseData)
                {
                    var rewardCode = reward.Key;
                    var rewardQuantity = int.Parse(reward.Value.ToString());

                    if (rewardCode.Equals(GSBackendKeys.PlayerDetails.GEMS))
                    {
                        playerModel.gems += rewardQuantity;
                    }
                    else if (rewardCode.Equals(GSBackendKeys.PlayerDetails.COINS))
                    {
                        playerModel.coins += rewardQuantity;
                    }
                    else if (playerModel.inventory.ContainsKey(rewardCode))
                    {
                        playerModel.inventory[rewardCode] += rewardQuantity;
                    }
                    else
                    {
                        playerModel.inventory.Add(rewardCode, rewardQuantity);
                    }
                }
            }
        }
    }

    #region REQUEST

    public class GSClaimRewardRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "ClaimReward";
        const string ATT_REWARD_JSON_DATA = "jsonData";

        public GSClaimRewardRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(GSRequestData jsonData, Action<object, Action<object>> onSuccess)
        {
            this.onSuccess = onSuccess;
            this.errorCode = BackendResult.CLAIM_REWARD_REQUEST_FAILED;

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_REWARD_JSON_DATA, jsonData)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
