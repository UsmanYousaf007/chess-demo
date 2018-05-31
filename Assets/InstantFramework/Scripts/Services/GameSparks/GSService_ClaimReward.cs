/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> ClaimReward(string rewardType)
        {
            return new GSClaimRewardRequest().Send(rewardType, OnClaimRewardSuccess);
        }

        private void OnClaimRewardSuccess(LogEventResponse response)
        {
            var res = response.ScriptData.GetGSData("rewardInfo");
            long bucks = res.GetInt("bucks").Value;

            playerModel.currency2 += bucks;
        }
    }
}
