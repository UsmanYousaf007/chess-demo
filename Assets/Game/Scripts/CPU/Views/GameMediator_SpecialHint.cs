using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.CPU
{
    public partial class GameMediator
    {
        //Dispatch Signals
        [Inject] public VirtualGoodsTransactionSignal virtualGoodsTransactionSignal { get; set; }

        //Listeners
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        private void OnRegisterSpecialHint()
        {
            view.InitSpecialHint();
            view.specialHintClickedSignal.AddListener(OnGetSpecialHint);
            view.notEnoughGemsSignal.AddListener(OnNotEnoughGems);
        }

        private void OnGetSpecialHint(VirtualGoodsTransactionVO vo)
        {
            virtualGoodsTransactionResultSignal.AddOnce(OnSpecialHintConsumed);
            virtualGoodsTransactionSignal.Dispatch(vo);
        }

        private void OnSpecialHintConsumed(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                preferencesModel.cpuPowerUpsUsedCount++;
                view.UpdateSpecialHintButton(preferencesModel.cpuPowerUpsUsedCount);
                getHintSignal.Dispatch(true);
            }
            else
            {
                OnCancelSpecialHint();
            }
        }

        [ListensTo(typeof(RenderHintSignal))]
        public void OnRenderSpecialHint(HintVO vo)
        {
            if (view.IsVisible() && vo.isHindsight)
            {
                view.RenderSpecialHint(vo);
            }
        }

        [ListensTo(typeof(SetupSpecialHintSignal))]
        public void OnSetupSpecialHint(SpecialHintVO vo)
        {
            if (view.IsVisible())
            {
                view.SetupSpecialHintButton(vo);
            }
        }

        [ListensTo(typeof(SpecialHintAvailableSignal))]
        public void OnSpecialHintAvailable(bool available)
        {
            if (view.IsVisible())
            {
                view.ToggleSpecialHintButton(available);
            }
        }

        [ListensTo(typeof(TurnSwapSignal))]
        public void OnToggleSpecialHintButton(bool isPlayerTurn)
        {
            if (view.IsVisible())
            {
                view.ToggleSpecialHintButton(isPlayerTurn);
            }
        }

        [ListensTo(typeof(CancelHintSingal))]
        public void OnCancelSpecialHint()
        {
            view.CancelSpecialHint();
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO inventory)
        {
            if (view.IsVisible())
            {
                view.SetupSpecialHintButton();
            }
        }

        private void OnNotEnoughGems()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
        }
    }
}
