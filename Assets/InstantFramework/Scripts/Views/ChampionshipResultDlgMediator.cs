using UnityEngine;
using strange.extensions.mediation.impl;
using System.Linq;

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
        [Inject] public StartLobbyChampionshipTimerSignal startLobbyChampionshipTimerSignal { get; set; }
        [Inject] public GetProfilePictureSignal getProfilePictureSignal { get; set; }
        [Inject] public UpdateTrophiesSignal updateTrophiesSignal { get; set; }

        private RewardDlgVO _rewardVO;
        private bool _playerRewarded = false;

        public override void OnRegister()
        {
            view.Init();
            view.loadPictureSignal.AddListener(OnLoadPicture);
            view.continueBtnClickedSignal.AddListener(OnContinuePressed);
        }

        [ListensTo(typeof(UpdateTournamentsViewSignal))]
        public void UpdateView()
        {
            if (view.gameObject.activeInHierarchy)
            {
                if (_rewardVO != null)
                {
                    JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament(_rewardVO.tournamentId);
                    if (joinedTournament != null && joinedTournament.entries.Count > 0)
                    {
                        view.UpdateView(playerModel.id, joinedTournament);
                    }
                }
            }
        }

        private void OnLoadPicture(GetProfilePictureVO vo)
        {
            getProfilePictureSignal.Dispatch(vo);
        }

        [ListensTo(typeof(ProfilePictureLoadedSignal))]
        public void OnPictureLoaded(string playerId, Sprite picture)
        {
            if (view.isActiveAndEnabled)
            {
                view.UpdatePicture(playerId, picture);
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

            getChampionshipTournamentLeaderboardSignal.Dispatch(_rewardVO.tournamentId, false);

            bool playerRewarded = false;
            for (int i = 0; i < _rewardVO.rewardQty.Count; i++)
            {
                if (_rewardVO.rewardQty[i] > 0)
                {
                    playerRewarded = true;
                    break;
                }
            }

            view.UpdateContinueButtonText(playerRewarded);

            _playerRewarded = playerRewarded;
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAMPIONSHIP_RESULT_DLG)
            {
                view.UpdateLeagueTitle(playerModel, tournamentsModel);
                view.Show();
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

            if (_playerRewarded)
            {
                // Dispatch rewards sequence signal here
                RewardDlgV2VO rewardDlgVO = new RewardDlgV2VO(_rewardVO, false);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REWARD_DLG_V2);
                updateRewardDlgViewSignal.Dispatch(rewardDlgVO);
                _playerRewarded = false;
            }

            JoinedTournamentData joinedTournamentData = tournamentsModel.GetJoinedTournament(_rewardVO.tournamentId);

            if (joinedTournamentData != null)
            {
                LogTournamentEndAnalytics(joinedTournamentData);

                tournamentsModel.RemoveFromJoinedTournament(_rewardVO.tournamentId);
            }

            startLobbyChampionshipTimerSignal.Dispatch();
            //updateTrophiesSignal.Dispatch(playerModel.trophies2 + _rewardVO.trophiesCount);
        }

        private void LogTournamentEndAnalytics(JoinedTournamentData data)
        {
            var earnedTrophies = data.rewardsDict[data.rank] == null ? 0 : data.rewardsDict[data.rank].trophies;
            var leagueName = leaguesModel.GetCurrentLeagueInfo().name.Replace(" ", "_").Replace(".", string.Empty).ToLower();
            analyticsService.Event($"{AnalyticsEventId.championship_finish_rank}_{leagueName}", AnalyticsParameter.context, GetRankContext(data.rank));

            if (earnedTrophies > 0)
            {
                analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.TROPHIES, earnedTrophies, "championship_reward", $"rank{data.rank}_{leagueName}");
            }

            var playerEntry = (from entry in data.entries
                               where entry.publicProfile.playerId.Equals(playerModel.id)
                               select entry).FirstOrDefault();

            if (playerEntry != null)
            {
                analyticsService.ValueEvent(AnalyticsEventId.championship_coins_by_rank, playerEntry.rank.ToString(), playerEntry.score);
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
