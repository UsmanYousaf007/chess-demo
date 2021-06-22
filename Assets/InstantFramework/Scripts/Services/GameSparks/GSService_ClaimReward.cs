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
        [Inject] public PowerPlayRewardClaimedSignal powerPlayRewardClaimedSignal { get; set; }
        [Inject] public FullAnalysisBoughtSignal fullAnalysisBoughtSignal { get; set; }

        public IPromise<BackendResult> ClaimReward(GSRequestData jsonData)
        {
            return new GSClaimRewardRequest(GetRequestContext()).Send(jsonData, OnClaimRewardSuccess, OnClaimRewardFailed);
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
                    var eloChange = GSParser.GetSafeInt(data, GSBackendKeys.Rewards.RATING_BOOST);
                    var gemsCosumed = GSParser.GetSafeInt(data, GSBackendKeys.PlayerDetails.GEMS);
                    UpdateElo(eloChange, gemsCosumed);
                    playerModel.gems -= gemsCosumed;
                }
                else if (rewardType.Equals(GSBackendKeys.ClaimReward.TYPE_PERSONALISED_ADS_GEM))
                {
                    GSData rewardData = data.GetGSData("reward");
                    var rewardQuantity = GSParser.GetSafeInt(rewardData, GSBackendKeys.PlayerDetails.GEMS);
                    playerModel.gems += rewardQuantity;
                    analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.GEMS, rewardQuantity, "new_player", "gdpr_accepted");
                }
                else if (rewardType.Equals(GSBackendKeys.ClaimReward.TYPE_RV_RATING_BOOSTER))
                {
                    playerModel.rvUnlockTimestamp = GSParser.GetSafeLong(data, GSBackendKeys.PlayerDetails.RV_UNLOCK_TIMESTAMP);
                    int eloChange = GSParser.GetSafeInt(data, GSBackendKeys.Rewards.RV_RATING_BOOST);
                    UpdateElo(eloChange, 0);
                }
                else if (rewardType.Equals(GSBackendKeys.ClaimReward.TYPE_RV_ANALYSIS))
                {
                    playerModel.rvUnlockTimestamp = GSParser.GetSafeLong(data, GSBackendKeys.PlayerDetails.RV_UNLOCK_TIMESTAMP);
                    fullAnalysisBoughtSignal.Dispatch();
                }
                else if (rewardType.Equals(GSBackendKeys.ClaimReward.TYPE_POWERPLAY))
                {
                    playerModel.rvUnlockTimestamp = GSParser.GetSafeLong(data, GSBackendKeys.PlayerDetails.RV_UNLOCK_TIMESTAMP);
                    powerPlayRewardClaimedSignal.Dispatch();
                }
                else
                {
                    var chestReward = ParseRewards(data.GetGSData(GSBackendKeys.ClaimReward.REWARD_INFO), rewardType);

                    if (rewardType.Equals(GSBackendKeys.ClaimReward.TYPE_LOBBY_CHEST_V2))
                    {
                        playerModel.chestUnlockTimestamp = GSParser.GetSafeLong(data, GSBackendKeys.PlayerDetails.CHEST_UNLOCK_TIMESTAMP);
                        lobbyChestRewardClaimedSignal.Dispatch(chestReward);
                    }
                }

                updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            }
        }

        private void OnClaimRewardFailed(object r)
        {
            var response = (LogEventResponse)r;
            var errorData = response.Errors;
            var errorString = errorData.GetString("error");

            switch(errorString)
            {
                case "invalidCoinPurchaseReward":
                    playerModel.coins = GSParser.GetSafeInt(errorData, "coins");
                    updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
                    navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
                    break;

                case "invalidChestReward":
                    playerModel.coins = GSParser.GetSafeInt(errorData, "coins");
                    playerModel.gems = GSParser.GetSafeInt(errorData, "gems");
                    playerModel.chestUnlockTimestamp = GSParser.GetSafeLong(errorData, GSBackendKeys.PlayerDetails.CHEST_UNLOCK_TIMESTAMP);
                    updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
                    lobbySequenceEndedSignal.Dispatch();
                    break;

                case "invalidRVReward":
                    playerModel.gems = GSParser.GetSafeInt(errorData, "gems");
                    playerModel.rvUnlockTimestamp = GSParser.GetSafeLong(errorData, GSBackendKeys.PlayerDetails.RV_UNLOCK_TIMESTAMP);
                    updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());

                    var rewardType = GSParser.GetSafeString(errorData, GSBackendKeys.ClaimReward.CLAIM_REWARD_TYPE);
                    var rvEnabled =
                        playerModel.gems < adsSettingsModel.minGemsRequiredforRV &&
                        playerModel.rvUnlockTimestamp > 0 &&
                        !(adsSettingsModel.removeRVOnPurchase && playerModel.HasPurchased());
                    
                    if (rewardType.Equals(GSBackendKeys.ClaimReward.TYPE_RV_RATING_BOOSTER))
                    {
                        playerModel.eloScore = GSParser.GetSafeInt(errorData, GSBackendKeys.PlayerDetails.ELO_SCORE);
                        rvEnabled = rvEnabled && adsSettingsModel.CanShowAdWithAdPlacement(AdPlacements.RV_rating_booster.ToString());
                    }
                    else if (rewardType.Equals(GSBackendKeys.ClaimReward.TYPE_RV_ANALYSIS))
                    {
                        rvEnabled = rvEnabled && adsSettingsModel.CanShowAdWithAdPlacement(AdPlacements.Rewarded_analysis.ToString());
                    }
                    
                    updateRVTimer.Dispatch(playerModel.rvUnlockTimestamp, rvEnabled);
                    break;

                case "invalidDailyReward":
                    playerModel.coins = GSParser.GetSafeInt(errorData, "coins");
                    playerModel.gems = GSParser.GetSafeInt(errorData, "gems");

                    if (inboxModel.items.ContainsKey("RewardDailyLeague")) {
                        var inboxItem = inboxModel.items["RewardDailyLeague"];
                        inboxItem.startTime = GSParser.GetSafeLong(errorData, "msgStartTime");
                        inboxItem.timeStamp = inboxItem.startTime;
                    }

                    dailyRewardClaimFailedSignal.Dispatch();
                    break;
            }
        }

        private void UpdateElo(int eloChange, int gemsCosumed)
        {
            var vo = new EloVO();
            playerModel.eloScore += eloChange;
            vo.playerEloScore = playerModel.eloScore;
            updateEloScoresSignal.Dispatch(vo);
            ratingBoostedSignal.Dispatch(eloChange, gemsCosumed);
        }

        private int ParseRewards(GSData data, string rewardType)
        {
            int rv = 0;

            if (data != null)
            {
                foreach (var reward in data.BaseData)
                {
                    var rewardCode = reward.Key;
                    var rewardQuantity = int.Parse(reward.Value.ToString());

                    if (rewardCode.Equals(GSBackendKeys.PlayerDetails.GEMS))
                    {
                        playerModel.gems += rewardQuantity;
                        rv = rewardQuantity;
                    }
                    else if (rewardCode.Equals(GSBackendKeys.PlayerDetails.COINS))
                    {
                        playerModel.coins += rewardQuantity;
                        rv = rewardQuantity;
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
                        case GSBackendKeys.ClaimReward.TYPE_LOBBY_CHEST_V2:
                            itemId = "lobby_chest";
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

            return rv;
        }
    }

    #region REQUEST

    public class GSClaimRewardRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "ClaimReward";
        const string ATT_REWARD_JSON_DATA = "jsonData";

        public GSClaimRewardRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(GSRequestData jsonData, Action<object, Action<object>> onSuccess, Action<object> onFailure)
        {
            this.onSuccess = onSuccess;
            this.onFailure = onFailure;
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
