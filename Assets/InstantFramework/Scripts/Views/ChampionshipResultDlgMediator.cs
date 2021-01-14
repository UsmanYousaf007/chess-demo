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
        [Inject] public ILeaguesModel leaguesModel { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateRewardDlgV2ViewSignal updateRewardDlgViewSignal { get; set; }
        [Inject] public ResetTournamentsViewSignal resetTournamentsViewSignal { get; set; }
        [Inject] public GetTournamentLeaderboardSignal getChampionshipTournamentLeaderboardSignal { get; set; }

        private RewardDlgVO _rewardVO;

        public override void OnRegister()
        {
            view.Init();

            view.continueBtnClickedSignal.AddListener(OnContinuePressed);
        }

        [ListensTo(typeof(UpdateTournamentsViewSignal))]
        public void UpdateView()
        {
            if (view.gameObject.activeInHierarchy)
            {
                JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament();
                if (joinedTournament != null && joinedTournament.entries.Count > 0)
                {
                    view.UpdateView(playerModel.id, joinedTournament);
                }
                else
                {
                    getChampionshipTournamentLeaderboardSignal.Dispatch(joinedTournament.id, false);
                }
            }
        }

        //[ListensTo(typeof(ResetTournamentsViewSignal))]
        //public void ResetView()
        //{
        //    view.ResetView();
        //}

        [ListensTo(typeof(UpdateChampionshipResultDlgSignal))]
        public void UpdateReward(RewardDlgVO vo)
        {
            _rewardVO = vo;
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAMPIONSHIP_RESULT_DLG)
            {
                JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament();
                if (joinedTournament == null)
                {
                    backendService.TournamentsOpGetJoinedTournaments();
                }
                else if (joinedTournament != null && joinedTournament.entries.Count > 0)
                {
                    view.UpdateView(playerModel.id, joinedTournament);
                }
                else
                {
                    getChampionshipTournamentLeaderboardSignal.Dispatch(joinedTournament.id, false);
                }

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
                view.ResetView();
            }
        }

        public void OnContinuePressed()
        {
            audioService.Play(audioService.sounds.SFX_CLICK);

            view.Hide();
            backendService.InBoxOpCollect(_rewardVO.msgId);
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            _rewardVO.onCloseSignal?.Dispatch();

            // Dispatch rewards sequence signal here
            RewardDlgV2VO rewardDlgVO = new RewardDlgV2VO(_rewardVO, false);
            updateRewardDlgViewSignal.Dispatch(rewardDlgVO);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REWARD_DLG_V2);

            JoinedTournamentData joinedTournamentData = tournamentsModel.GetJoinedTournament();
            tournamentsModel.RemoveFromJoinedTournament(joinedTournamentData.id);

            LogTournamentEndAnalytics(joinedTournamentData);

            backendService.TournamentsOpGetJoinedTournaments();
        }

        private void LogTournamentEndAnalytics(JoinedTournamentData data)
        {
            var earnedTrophies = data.rewardsDict[data.rank].trophies;
            var leagueName = leaguesModel.GetCurrentLeagueInfo().name.Replace(" ", "_").Replace(".", string.Empty).ToLower();
            analyticsService.Event($"{AnalyticsEventId.championship_finish_rank}_{leagueName}", AnalyticsParameter.context, GetRankContext(data.rank));

            if (earnedTrophies > 0)
            {
                analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.TROPHIES, earnedTrophies, "championship_reward", $"rank{data.rank}_{leagueName}");
            }
        }

        private string GetRankContext(int rank)
        {
            if (rank > 50)
            {
                return "51_to_100";
            }
            else if (rank <= 50 && rank >= 11)
            {
                return "11_to_50";
            }
            else if (rank <= 10 && rank >= 4)
            {
                return "4_to_10";
            }

            return rank.ToString();
        }
    }
}
