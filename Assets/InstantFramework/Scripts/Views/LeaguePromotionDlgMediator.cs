﻿using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class LeaguePromotionDlgMediator : Mediator
    {
        //View Injection
        [Inject] public LeaguePromotionDlgView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ILeaguesModel leaguesModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateRewardDlgV2ViewSignal updateRewardDlgViewSignal { get; set; }

        private RewardDlgVO _rewardVO;

        public override void OnRegister()
        {
            view.Init();

            view.CollectBtnClickedSignal.AddListener(OnCollectPressed);
        }

        [ListensTo(typeof(UpdateLeaguePromotionDlgViewSignal))]
        public void UpdateView(RewardDlgVO vo)
        {
            _rewardVO = vo;
            view.UpdateLeagueTitle(playerModel, tournamentsModel);
            view.UpdateView(vo, leaguesModel.GetCurrentLeagueInfo().dailyReward);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LEAGUE_PROMOTION_DLG)
            {
                view.Show();
                //analyticsService.ScreenVisit(AnalyticsScreen.inventory);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LEAGUE_PROMOTION_DLG)
            {
                view.Hide();
            }
        }

        private void OnCollectPressed()
        {
            audioService.Play(audioService.sounds.SFX_CLICK);

            view.Hide();
            backendService.InBoxOpCollect(_rewardVO.msgId);
            _rewardVO.onCloseSignal?.Dispatch();

            // Dispatch rewards sequence signal here
            RewardDlgV2VO rewardDlgVO = new RewardDlgV2VO(_rewardVO, false);
            updateRewardDlgViewSignal.Dispatch(rewardDlgVO);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REWARD_DLG_V2);
        }
    }
}
