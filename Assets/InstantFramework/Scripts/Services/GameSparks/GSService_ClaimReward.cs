/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> ClaimReward(string rewardType)
        {
            return new GSClaimRewardRequest(rewardType, OnClaimRewardSuccess).Send();
        }

        private void OnClaimRewardSuccess(LogEventResponse response)
        {
            var res = response.ScriptData.GetGSData("rewardInfo");
            long bucks = res.GetInt("bucks").Value;

            playerModel.bucks += bucks;
        }
    }

    #region REQUEST

    public class GSClaimRewardRequest : GSLogEventRequest
    {
        public GSClaimRewardRequest(string rewardType, Action<LogEventResponse> onSuccess)
        {
            // Set your request parameters here
            key = "ClaimReward";
            request.SetEventAttribute("rewardType", rewardType);
            errorCode = BackendResult.GET_INIT_DATA_REQUEST_FAILED;

            // Do not modify below
            this.onSuccess = onSuccess;
        }
    }

    #endregion
}
