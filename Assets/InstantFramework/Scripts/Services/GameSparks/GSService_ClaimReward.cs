/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> ClaimReward(GSRequestData jsonData)
        {
            return new GSClaimRewardRequest().Send(jsonData, OnClaimRewardSuccess);
        }

        private void OnClaimRewardSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;
            if (response != null && response.ScriptData != null)
            {
                if(response.ScriptData.ContainsKey(GSBackendKeys.ClaimReward.REWARD_INFO))
                {
                    var res = response.ScriptData.GetGSData(GSBackendKeys.ClaimReward.REWARD_INFO);
                    
                    playerModel.bucks += res.GetInt(GSBackendKeys.ClaimReward.BUCKS).Value;
                }

                if (response.ScriptData.ContainsKey(GSBackendKeys.PlayerDetails.CPU_POWERUP_USED_COUNT))
                {
                    playerModel.cpuPowerupUsedCount = response.ScriptData.GetInt(GSBackendKeys.PlayerDetails.CPU_POWERUP_USED_COUNT).Value;
                }
            }
               
        }
    }

    #region REQUEST

    public class GSClaimRewardRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "ClaimReward";
        const string ATT_REWARD_JSON_DATA = "jsonData";

        public IPromise<BackendResult> Send(GSRequestData jsonData, Action<object> onSuccess)
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
