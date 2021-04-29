using System.Collections;
using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ChampionshipNewRankDlgMediator : Mediator
    {
        //View Injection
        [Inject] public ChampionshipNewRankDlgView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IRoutineRunner routineRunner { get; set; }

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public GetTournamentLeaderboardSignal getChampionshipTournamentLeaderboardSignal { get; set; }
        [Inject] public RankPromotedDlgClosedSignal rankPromotedDlgClosedSignal { get; set; }
        [Inject] public StartLobbyChampionshipTimerSignal startLobbyChampionshipTimerSignal { get; set; }
        [Inject] public GetProfilePictureSignal getProfilePictureSignal { get; set; }
        [Inject] public ShowAdSignal showAdSignal { get; set; }
        [Inject] public ResetCareerprogressionViewSignal resetCareerprogressionViewSignalshowAdSignal { get; set; }

        // Listeners
        [Inject] public UpdateTournamentLeaderboardSignal updateTournamentLeaderboardSignal { get; set; }

        private int oldRank = -1;

        public override void OnRegister()
        {
            view.Init();
            view.loadPictureSignal.AddListener(OnLoadPicture);
            view.continueButtonClickedSignal.AddListener(OnContinuePressed);
        }

        private void OnLoadPicture(GetProfilePictureVO vo)
        {
            getProfilePictureSignal.Dispatch(vo);
        }

        //[ListensTo(typeof(ResetTournamentsViewSignal))]
        //public void ResetView()
        //{
        //    view.ResetView();
        //}

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAMPIONSHIP_NEW_RANK_DLG)
            {
                if (matchInfoModel.lastCompletedMatch.isRanked)
                {
                    JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament();
                    view.Show(joinedTournament, oldRank != -1 && joinedTournament.rank > oldRank);
                    oldRank = joinedTournament.rank;

                    if (joinedTournament != null && joinedTournament.entries.Count > 0)
                    {
                        view.UpdateView(playerModel.id, joinedTournament);
                    }
                    else
                    {
                        view.ShowProcessing();
                        routineRunner.StartCoroutine(GetLeaderboardDataAsync(joinedTournament));
                        updateTournamentLeaderboardSignal.AddOnce(OnLeaderboardDataLoaded);
                    }

                    view.UpdateLeagueTitle(playerModel, tournamentsModel);
                    //analyticsService.ScreenVisit(AnalyticsScreen.inventory);
                }
                else
                {
                    OnContinuePressed(string.Empty, false);
                }
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAMPIONSHIP_NEW_RANK_DLG)
            {
                view.Hide();
                view.ResetView();
                rankPromotedDlgClosedSignal.Dispatch();
            }
        }

        private void OnContinuePressed(string challengeId, bool playerWins)
        {
            audioService.Play(audioService.sounds.SFX_CLICK);

            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            startLobbyChampionshipTimerSignal.Dispatch();
            resetCareerprogressionViewSignalshowAdSignal.Dispatch();
            ShowInterstitialOnBack(AnalyticsContext.interstitial_endgame, AdPlacements.Interstitial_endgame, challengeId, playerWins);
        }

        private void ShowInterstitialOnBack(AnalyticsContext analyticsContext, AdPlacements placementId, string challengeId, bool playerWins)
        {
            ResultAdsVO vo = new ResultAdsVO();
            vo.adsType = AdType.Interstitial;
            vo.rewardType = GSBackendKeys.ClaimReward.NONE;
            vo.challengeId = challengeId;
            vo.playerWins = playerWins;
            vo.placementId = placementId;
            playerModel.adContext = analyticsContext;

            showAdSignal.Dispatch(vo, false);
        }

        [ListensTo(typeof(ProfilePictureLoadedSignal))]
        public void OnPictureLoaded(string playerId, Sprite picture)
        {
            if (view.isActiveAndEnabled)
            {
                view.UpdatePicture(playerId, picture);
            }
        }

        [ListensTo(typeof(UpdateNewRankChampionshipDlgViewSignal))]
        public void UpdateView(string challengeId, bool playerWins, float duration)
        {
            view.UpdateView(challengeId, playerWins, duration);
        }

        private IEnumerator GetLeaderboardDataAsync(JoinedTournamentData joinedTournament)
        {
            yield return new WaitForEndOfFrame();
            getChampionshipTournamentLeaderboardSignal.Dispatch(joinedTournament.id, false);
        }

        public void OnLeaderboardDataLoaded(string tournamentId)
        {
            JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament(tournamentId);
            if (joinedTournament != null && joinedTournament.entries.Count > 0)
            {
                view.UpdateView(playerModel.id, joinedTournament);
            }
        }
    }
}
