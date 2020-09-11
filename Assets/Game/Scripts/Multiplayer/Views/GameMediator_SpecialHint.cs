using GameAnalyticsSDK;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public partial class GameMediator
    {
        private VirtualGoodsTransactionVO transactionVO;

        private void OnRegisterSpecialHint()
        {
            view.InitSpecialHint();
            view.specialHintClickedSignal.AddListener(OnGetSpecialHint);
        }

        private void OnGetSpecialHint(VirtualGoodsTransactionVO vo)
        {
            if (chessboardModel.isValidChallenge(matchInfoModel.activeChallengeId))
            {
                transactionVO = vo;
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

                if (!transactionVO.consumeItemShortCode.Equals("premium"))
                {
                    analyticsService.ResourceEvent(GAResourceFlowType.Sink, CollectionsUtil.GetContextFromString(transactionVO.consumeItemShortCode).ToString(), transactionVO.consumeQuantity, "booster_used", "hint");
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
    }
}
