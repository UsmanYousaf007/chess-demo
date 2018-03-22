/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-09 14:51:11 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public class Currency1RewardModalMediator : Mediator
    {
        // View injection
        [Inject] public Currency1RewardModalView view { get; set; }

        [Inject] public CloseModalViewSignal closeModalViewSignal { get; set; }

        public override void OnRegister()
        {
            view.claimRewardButtonClickedSignal.AddListener(OnClaimRewardButtonClicked);
            view.Init();
        }

        public override void OnRemove()
        {
            // TODO(mubeeniqbal): For all views add a Cleanup() method which will be called from OnRemove().

            view.claimRewardButtonClickedSignal.RemoveListener(OnClaimRewardButtonClicked);
        }

        [ListensTo(typeof(UpdateCurrency1RewardModalViewSignal))]
        public void OnUpdateView(Currency1RewardModalVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateCurrency1RewardModalViewShowWaitForRewardSignal))]
        public void OnShowWaitForReward()
        {
            view.ShowWaitForReward();
        }

        [ListensTo(typeof(ShowModalViewSignal))]
        public void OnShowView(ModalViewId modalViewId)
        {
            if (modalViewId == ModalViewId.CURRENCY_1_REWARD) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(HideModalViewSignal))]
        public void OnHideView(ModalViewId modalViewId)
        {
            if (modalViewId == ModalViewId.CURRENCY_1_REWARD)
            {
                view.Hide();
            }
        }

        private void OnClaimRewardButtonClicked()
        {
            closeModalViewSignal.Dispatch();
        }
    }
}
