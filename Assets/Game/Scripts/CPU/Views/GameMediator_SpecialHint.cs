using GameAnalyticsSDK;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

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

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        private VirtualGoodsTransactionVO transactionVO;

        private void OnRegisterSpecialHint()
        {
            view.InitSpecialHint();
            view.specialHintClickedSignal.AddListener(OnGetSpecialHint);
            view.notEnoughGemsSignal.AddListener(OnNotEnoughGems);
        }

        private void OnGetSpecialHint(VirtualGoodsTransactionVO vo)
        {
            transactionVO = vo;

            if (vo.consumeItemShortCode.Equals("premium"))
            {
                OnSpecialHintConsumed(BackendResult.SUCCESS);
            }
            else
            {
                virtualGoodsTransactionResultSignal.AddOnce(OnSpecialHintConsumed);
                virtualGoodsTransactionSignal.Dispatch(vo);
            }
        }

        private void OnSpecialHintConsumed(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                preferencesModel.cpuPowerUpsUsedCount++;
                view.UpdateSpecialHintButton(preferencesModel.cpuPowerUpsUsedCount);
                getHintSignal.Dispatch(true);

                if (!transactionVO.consumeItemShortCode.Equals("premium"))
                {
                    analyticsService.ResourceEvent(GAResourceFlowType.Sink, CollectionsUtil.GetContextFromString(transactionVO.consumeItemShortCode).ToString(), transactionVO.consumeQuantity, "booster_used", "hint");
                    preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_USED][transactionVO.consumeItemShortCode] += transactionVO.consumeQuantity;
                }
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
