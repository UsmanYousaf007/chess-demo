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
        [Inject] public RatingBoostedSignal ratingBoostedSignal { get; set; }
        [Inject] public LobbyChestRewardClaimedSignal lobbyChestRewardClaimedSignal { get; set; }

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

                if (rewardType.Equals(GSBackendKeys.ClaimReward.TYPE_BOOST_RATING))
                {
                    var vo = new EloVO();
                    var eloChange = GSParser.GetSafeInt(data, GSBackendKeys.Rewards.RATING_BOOST);
                    var gemsCosumed = GSParser.GetSafeInt(data, GSBackendKeys.PlayerDetails.GEMS);
                    playerModel.eloScore += eloChange;
                    vo.playerEloScore = playerModel.eloScore;
                    updateEloScoresSignal.Dispatch(vo);
                    ratingBoostedSignal.Dispatch(eloChange, gemsCosumed);
                    playerModel.gems -= gemsCosumed;
                }
                else if (rewardType.Equals(GSBackendKeys.ClaimReward.TYPE_PERSONALISED_ADS_GEM))
                {
                    GSData rewardData = data.GetGSData("reward");
                    var rewardQuantity = GSParser.GetSafeInt(rewardData, GSBackendKeys.PlayerDetails.GEMS);
                    playerModel.gems += rewardQuantity;
                    analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.GEMS, rewardQuantity, "new_player", "gdpr_accepted");
                }
                else
                {
                    var coinReward = ParseRewards(data.GetGSData(GSBackendKeys.ClaimReward.REWARD_INFO), rewardType);

                    if (rewardType.Equals(GSBackendKeys.ClaimReward.TYPE_LOBBY_CHEST))
                    {
                        playerModel.chestUnlockTimestamp = GSParser.GetSafeLong(data, GSBackendKeys.PlayerDetails.CHEST_UNLOCK_TIMESTAMP);
                        lobbyChestRewardClaimedSignal.Dispatch(coinReward);
                    }
                }

                updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            }
        }

        private int ParseRewards(GSData data, string rewardType)
        {
            int coinReward = 0;

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
                        coinReward = rewardQuantity;
                    }
                    else if (playerModel.inventory.ContainsKey(rewardCode))
                    {
                        playerModel.inventory[rewardCode] += rewardQuantity;
                    }
                    else
                    {
                        playerModel.inventory.Add(rewardCode, rewardQuantity);
                    }

                    //Analytics
                    var itemId = string.Empty;
                    var itemType = "rv_popup";

                    switch (rewardType)
                    {
                        case GSBackendKeys.ClaimReward.TYPE_LOBBY_CHEST:
                            itemId = "lobby_coins_chest_timer_end";
                            break;

                        case GSBackendKeys.ClaimReward.TYPE_DAILY:
                            var leagueName = leaguesModel.GetCurrentLeagueInfo().name.Replace(" ", "_").Replace(".", string.Empty).ToLower();
                            itemId = rewardCode.Equals(GSBackendKeys.PlayerDetails.GEMS) ? $"{leagueName}_double_gems" : "double_coins";
                            itemType = "daily_league_reward";
                            break;

                        case GSBackendKeys.ClaimReward.TYPE_COINS_PURCHASE:
                            itemId = playerModel.adContext.ToString().Replace("rewarded_", string.Empty);
                            break;
                    }

                    analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, CollectionsUtil.GetContextFromString(rewardCode).ToString(), rewardQuantity, itemType, itemId);
                    //Analytics End
                }
            }

            return coinReward;
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
