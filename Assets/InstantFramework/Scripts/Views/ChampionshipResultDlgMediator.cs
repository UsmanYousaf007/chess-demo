using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class ChampionshipResultDlgMediator : Mediator
    {
        //View Injection
        [Inject] public ChampionshipResultDlgView view { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateRewardDlgV2ViewSignal updateRewardDlgViewSignal { get; set; }
        [Inject] public ResetTournamentsViewSignal resetTournamentsViewSignal { get; set; }

        private RewardDlgVO _dailyRewardVO;

        public override void OnRegister()
        {
            view.Init();

            view.continueBtnClickedSignal.AddListener(OnContinuePressed);
        }

        [ListensTo(typeof(UpdateTournamentsViewSignal))]
        public void UpdateView()
        {
            view.UpdateView(playerModel.id, tournamentsModel.GetJoinedTournament());
        }

        [ListensTo(typeof(ResetTournamentsViewSignal))]
        public void ResetView()
        {
            view.ResetView();
        }

        [ListensTo(typeof(UpdateChampionshipResultDlgSignal))]
        public void UpdateReward(RewardDlgVO vo)
        {
            _dailyRewardVO = vo;
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAMPIONSHIP_RESULT_DLG)
            {
                view.UpdateRank(playerModel, tournamentsModel);
                view.Show();
                //analyticsService.ScreenVisit(AnalyticsScreen.inventory);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAMPIONSHIP_RESULT_DLG)
            {
                view.Hide();
            }
        }

        public void OnContinuePressed()
        {
            audioService.Play(audioService.sounds.SFX_CLICK);

            view.Hide();
            backendService.InBoxOpCollect(_dailyRewardVO.msgId);
            _dailyRewardVO.onCloseSignal?.Dispatch();

            // Dispatch rewards sequence signal here
            RewardDlgV2VO rewardDlgVO = CreateRewardDlgV2VO(false);
            updateRewardDlgViewSignal.Dispatch(rewardDlgVO);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REWARD_DLG_V2);

            tournamentsModel.RemoveFromJoinedTournament(tournamentsModel.GetJoinedTournament().id);
            resetTournamentsViewSignal.Dispatch();
            backendService.TournamentsOpGetJoinedTournaments();
        }

        private RewardDlgV2VO CreateRewardDlgV2VO(bool videoWatched)
        {
            RewardDlgV2VO rewardDlgVO = new RewardDlgV2VO();
            rewardDlgVO.RVWatched = videoWatched;
            for (int i = 0; i < _dailyRewardVO.rewardShortCodes.Count; i++)
            {
                int quantity = _dailyRewardVO.GetRewardItemQty(i);
                if (videoWatched)
                {
                    quantity *= 2;
                }
                rewardDlgVO.Rewards.Add(new RewardDlgV2VO.Reward(_dailyRewardVO.rewardShortCodes[i], quantity));
            }

            return rewardDlgVO;
        }
    }
}
