/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        string rewardTypeId;

        public IPromise<BackendResult> ClaimReward(string rewardType)
        {
            rewardTypeId = rewardType;
            return new GSClaimRewardRequest().Send(rewardType, OnClaimRewardSuccess);
        }

        private void OnClaimRewardSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;
            var res = response.ScriptData.GetGSData(GSBackendKeys.ClaimReward.REWARD_INFO);

            if (rewardTypeId == GSBackendKeys.ClaimReward.TYPE_AD_BUCKS)
            {
                playerModel.bucks += res.GetInt(GSBackendKeys.ClaimReward.BUCKS).Value;
                playerModel.adLifetimeImpressions = res.GetInt(GSBackendKeys.ClaimReward.AD_LIFETIME_IMPRESSIONS).Value;
            }
        }
    }

    #region REQUEST

    public class GSClaimRewardRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "ClaimReward";
        const string ATT_REWARD_TYPE = "rewardType";

        public IPromise<BackendResult> Send(string rewardType, Action<object> onSuccess)
        {
            this.onSuccess = onSuccess;
            this.errorCode = BackendResult.CLAIM_REWARD_REQUEST_FAILED;

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_REWARD_TYPE, rewardType)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
