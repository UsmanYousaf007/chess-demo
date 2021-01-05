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

                if (rewardType == GSBackendKeys.ClaimReward.TYPE_BOOST_RATING)
                {
                    EloVO vo = new EloVO();
                    playerModel.eloScore += GSParser.GetSafeInt(data, GSBackendKeys.Rewards.RATING_BOOST);
                    vo.playerEloScore = playerModel.eloScore;
                    updateEloScoresSignal.Dispatch(vo);
                    ratingBoostedSignal.Dispatch(GSParser.GetSafeInt(response.ScriptData, GSBackendKeys.Rewards.RATING_BOOST));
                    analyticsService.Event(AnalyticsEventId.booster_used, AnalyticsContext.rating_booster);
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
