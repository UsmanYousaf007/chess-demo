/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;

namespace TurboLabz.CPU
{
    public partial class GameMediator
    {
        //Dispatch Signals
        [Inject] public GetHintSignal getHintSignal { get; set; }
        [Inject] public IAdsService adsService { get; set; }

        private void OnRegisterHint()
        {
            view.InitHint();
            view.hintClickedSignal.AddListener(OnGetHint);
        }

        private void OnGetHint()
        {
            getHintSignal.Dispatch(false);
        }

        [ListensTo(typeof(RenderHintSignal))]
        public void OnRenderHint(HintVO vo)
        {
            if (!vo.isHindsight)
            {
                view.RenderHint(vo);
            }
        }

        [ListensTo(typeof(CancelHintSingal))]
        public void OnCancelHint()
        {
            view.CancelHint();
        }

        //[ListensTo(typeof(TurnSwapSignal))]
        public void OnToggleHintButton(bool isPlayerTurn)
        {
            view.ToggleHintButton(isPlayerTurn);
        }

        [ListensTo(typeof(UpdateHintCountSignal))]
        public void OnUpdateHintCount(int count)
        {
            if (view.IsVisible())
            {
                view.UpdateHintCount(count);
            }
        }

        [ListensTo(typeof(HintAvailableSignal))]
        public void OnHintAvailable(bool available)
        {
            if (available)
            {
                view.EnableHintButton();
            }
            else
            {
                view.DisableHintButton();
            }
        }

        [ListensTo(typeof(ShowStrengthOnboardingTooltipSignal))]
        public void OnShowOnboardTooltip(bool show)
        {
            view.ShowStrengthOnboardingTooltip(show);
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnSubscriptionPurchased(StoreItem item)
        {
            if (view.IsVisible())
            {
                view.UpdateHintCount(0);
                view.UpdateHindsightCount(0);
                adsService.HideBanner();
            }
        }
    }
}
