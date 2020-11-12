using GameAnalyticsSDK;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public partial class GameMediator
    {
        private VirtualGoodsTransactionVO hintTransactionVO;

        private void OnRegisterSpecialHint()
        {
            view.InitSpecialHint();
            view.specialHintClickedSignal.AddListener(OnGetSpecialHint);
            view.notEnoughSpecialHintsSingal.AddListener(OnNotEnoughSpeciallHints);
        }

        private void OnGetSpecialHint(VirtualGoodsTransactionVO vo)
        {
            if (chessboardModel.isValidChallenge(matchInfoModel.activeChallengeId))
            {
                hintTransactionVO = vo;
                vo.challengeId = matchInfoModel.activeChallengeId;
                virtualGoodsTransactionResultSignal.AddOnce(OnSpecialHintConsumed);
                virtualGoodsTransactionSignal.Dispatch(vo);
            }
        }

        private void OnSpecialHintConsumed(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                view.UpdateSpecialHintButton(matchInfoModel.activeMatch.playerPowerupUsedCount);
                getHintSignal.Dispatch(true);

                if (!hintTransactionVO.consumeItemShortCode.Equals("premium"))
                {
                    analyticsService.ResourceEvent(GAResourceFlowType.Sink, CollectionsUtil.GetContextFromString(hintTransactionVO.consumeItemShortCode).ToString(), hintTransactionVO.consumeQuantity, "booster_used", "hint");
                    preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_USED][hintTransactionVO.consumeItemShortCode] += hintTransactionVO.consumeQuantity;
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

        private void OnNotEnoughSpeciallHints(VirtualGoodsTransactionVO vo)
        {
            hintTransactionVO = vo;
            var spotInventoryParams = new LoadSpotInventoryParams();
            spotInventoryParams.itemShortCode = vo.consumeItemShortCode;
            spotInventoryParams.itemToUnclockShortCode = vo.consumeItemShortCode;
            spotInventoryParams.challengeId = matchInfoModel.activeChallengeId;
            loadSpotInventorySignal.Dispatch(spotInventoryParams);
        }
    }
}
