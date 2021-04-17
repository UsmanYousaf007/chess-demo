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
                var isPremium = hintTransactionVO.consumeItemShortCode.Equals("premium");
                if (preferencesModel.freeHint.HasFlag(FreePowerUpStatus.NOT_CONSUMED | FreePowerUpStatus.AVAILABLE))
                    preferencesModel.freeHint = FreePowerUpStatus.CONSUMED;

                // Ensure that match is still active (that game has not ended when consume hint success returns)
                if (matchInfoModel.activeMatch != null)
                {
                    view.UpdateSpecialHintButton(matchInfoModel.activeMatch.playerPowerupUsedCount, !isPremium, matchInfoModel.activeMatch.freeHints);
                    getHintSignal.Dispatch(true);

                    if (!isPremium)
                    {
                        analyticsService.Event(AnalyticsEventId.gems_used, AnalyticsContext.hint);
                        analyticsService.ResourceEvent(GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, hintTransactionVO.consumeQuantity, "booster_used", AnalyticsContext.hint.ToString());
                    }
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

        [ListensTo(typeof(FreeHintAvailableSignal))]
        public void OnFreeHintAvailable(bool isFreeHintAvailable)
        {
            if (view.IsVisible())
            {
                view.SetupSpecialHintButton();
            }
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
