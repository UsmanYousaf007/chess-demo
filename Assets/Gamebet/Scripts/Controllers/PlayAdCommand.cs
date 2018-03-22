/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-12 15:03:58 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class PlayAdCommand : Command
    {
        // Dispatch signals
        [Inject] public UpdateFreeCurrency1ModalViewSignal updateFreeCurrency1ModalViewSignal { get; set; }
        [Inject] public LoadModalViewSignal loadModalViewSignal { get; set; }
        [Inject] public UpdateCurrency1RewardModalViewShowWaitForRewardSignal updateCurrency1RewardModalViewShowWaitForRewardSignal { get; set; }
        [Inject] public UpdateCurrency1RewardModalViewSignal updateCurrency1RewardModalViewSignal { get; set; }
        [Inject] public UpdateOutOfCurrency1ModalViewSignal updateOutOfCurrency1ModalViewSignal { get; set; }

        // Models.
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAdInfoModel adInfoModel { get; set; }

        // Services.
        [Inject] public IAdsService adService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();
            adService.ShowAd().Then(OnShowAd);
            /*
            bool isAdAvailable = adService.isAdAvailable;
            updateFreeCurrency1ModalViewSignal.Dispatch(isAdAvailable);
            updateOutOfCurrency1ModalViewSignal.Dispatch(isAdAvailable);

            if (isAdAvailable)
            {
                Retain();
                adService.ShowAd().Then(OnShowAd);
            }
            */
        }

        private void OnShowAd(AdsResult result)
        {
            /*
            if (result == AdsResult.FINISHED)
            {
            }
            */
                loadModalViewSignal.Dispatch(ModalViewId.CURRENCY_1_REWARD);
                updateCurrency1RewardModalViewShowWaitForRewardSignal.Dispatch();

                backendService.ClaimAdReward().Then(OnClaimAdReward);
        }

        private void OnClaimAdReward(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                Currency1RewardModalVO vo;
                vo.currency1 = playerModel.currency1;
                vo.currency2 = playerModel.currency2;
                vo.currency1Reward = adInfoModel.reward.currency1;

                updateCurrency1RewardModalViewSignal.Dispatch(vo);

                playerModel.currency1 += adInfoModel.reward.currency1;
            }
        }
    }
}
