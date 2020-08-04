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
        [Inject] public RatingBoostAnimSignal ratingBoostAnimSignal { get; set; }

        public IPromise<BackendResult> ClaimReward(GSRequestData jsonData)
        {
            return new GSClaimRewardRequest(GetRequestContext()).Send(jsonData, OnClaimRewardSuccess);
        }

        private void OnClaimRewardSuccess(object r, Action<object> a)
        {
            LogEventResponse response = (LogEventResponse)r;
            if (response != null && response.ScriptData != null)
            {
                GSParser.PopulateAdsRewardData(playerModel, response.ScriptData);

                EloVO vo = new EloVO();
                vo.playerEloScore = playerModel.eloScore;
                updateEloScoresSignal.Dispatch(vo);

                ratingBoostAnimSignal.Dispatch(GSParser.GetSafeInt(response.ScriptData, GSBackendKeys.Rewards.RATING_BOOST));
                LogUtil.Log(string.Format("Found ads reward data index {0} current {1} required {2}", playerModel.rewardIndex, playerModel.rewardCurrentPoints, playerModel.rewardPointsRequired));
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
