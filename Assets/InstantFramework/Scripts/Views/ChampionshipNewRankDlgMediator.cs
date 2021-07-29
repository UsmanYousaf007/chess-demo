using System.Collections;
﻿using System;
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
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public ISchedulerService schedulerService { get; set; }

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

        private int oldRank = -1;

        public override void OnRegister()
        {
            view.serverClock = backendService.serverClock;

            view.Init();
            view.loadPictureSignal.AddListener(OnLoadPicture);
            view.continueButtonClickedSignal.AddListener(OnContinuePressed);
            view.schedulerSubscription.AddListener(OnSchedulerSubscriptionToggle);
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

        // NOTE: Do not update on signal. New Rank dialog does not need to refresh when it is
        // already open.
        //[ListensTo(typeof(UpdateTournamentsViewSignal))]
        public void UpdateView()
        {
            if (view.gameObject.activeInHierarchy)
            {
                view.UpdateView(playerModel.id, tournamentsModel.GetJoinedTournament());
            }
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAMPIONSHIP_NEW_RANK_DLG)
            {
                if (matchInfoModel.lastCompletedMatch.isRanked)
                {
                    view.Show();
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

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);
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

        [ListensTo(typeof(UpdateTournamentLeaderboardSignal))]
        public void OnLeaderboardDataLoaded(string tournamentId)
        {
            JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament(tournamentId);
            if (joinedTournament != null && joinedTournament.entries.Count > 0)
            {
                view.UpdateView(playerModel.id, joinedTournament);
            }
        }

        private void OnSchedulerSubscriptionToggle(Action callback, bool subscribe)
        {
            if (subscribe)
            {
                schedulerService.Subscribe(callback);
            }
            else
            {
                schedulerService.UnSubscribe(callback);
            }
        }

    }
}
