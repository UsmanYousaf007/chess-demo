/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-13 12:20:08 UTC+05:00
/// 
/// @description
/// [add_description_here]

using GameSparks.Api.Responses;
using strange.extensions.promise.api;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        // Models.
        [Inject] public IAdInfoModel AdInfoModel { get; set; }

        public IPromise<BackendResult> ClaimAdReward()
        {
            return new GSClaimAdRewardRequest().Send(OnClaimAdRewardSuccess);
        }

        private void OnClaimAdRewardSuccess(LogEventResponse response)
        {
            // TODO: Temporary code to patch apparent missing git commit code

            var res = response.ScriptData.GetGSData("adInfo");
            var rewardres = res.GetGSData("reward");
            var coins = rewardres.GetInt("coins").Value;

            TurboLabz.Common.LogUtil.Log("***************** REWARD COINS " + coins);

            long currency1 = coins;
            //long currency1 = response.ScriptData.GetGSData("adReward");

            AdReward reward;
            reward.currency1 = currency1;
            reward.currency2 = 0;

            AdInfoModel.reward = reward;
        }
    }
}
