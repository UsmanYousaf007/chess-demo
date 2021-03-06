using System;
using strange.extensions.mediation.impl;
/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class LobbyProfileBarMediator : Mediator
    {
        // View injection
        [Inject] public LobbyProfileBarView view { get; set; }

        //Dispatch signals
        [Inject] public GetTournamentLeaderboardSignal fetchLeaderboardSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ShowRewardedAdSignal showRewardedAdSignal { get; set; }
        [Inject] public UpdateRewardDlgV2ViewSignal updateRewardDlgViewSignal { get; set; }
        [Inject] public LoadCareerCardSignal loadCareerCardSignal { get; set; }

        //Services
        [Inject] public ISchedulerService schedulerService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        private int chestReward;

        public override void OnRegister()
        {
            view.Init();
            view.leaderboardButtonClickedSignal.AddListener(OnLeaderboardClicked);
            view.chestButtonClickedSignal.AddListener(OnChestClicked);
            view.schedulerSubscription.AddListener(OnSchedulerSubscription);

            view.serverClock = backendService.serverClock;
        }

        [ListensTo(typeof(UpdateProfileSignal))]
        public void OnUpdateProfile(ProfileVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateTrophiesSignal))]
        public void OnUpdateTrophies(int trophies)
        {
            view.UpdateTrophies(trophies);
        }

        [ListensTo(typeof(AuthFacebookResultSignal))]
        public void OnAuthFacebookResult(AuthFacebookResultVO vo)
        {
            if (view.isActiveAndEnabled)
            {
                var profileVO = new ProfileVO();
                profileVO.playerName = vo.name;
                profileVO.eloScore = vo.rating;
                profileVO.countryId = vo.countryId;
                profileVO.trophies2 = vo.trophies2;
                view.UpdateView(profileVO);
            }
        }

        [ListensTo(typeof(AuthSignInWithAppleResultSignal))]
        public void OnAuthSignInWithAppleSignal(AuthSignInWIthAppleResultVO vo)
        {
            if (view.isActiveAndEnabled)
            {
                var profileVO = new ProfileVO();
                profileVO.playerName = vo.name;
                profileVO.eloScore = vo.rating;
                profileVO.countryId = vo.countryId;
                profileVO.trophies2 = vo.trophies2;
                view.UpdateView(profileVO);
            }
        }

        [ListensTo(typeof(UpdateEloScoresSignal))]
        public void OnUpdateEloScoresSignal(EloVO vo)
        {
            view.UpdateEloScores(vo);
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY)
            {
                view.Hide();
            }
        }

        private void OnLeaderboardClicked()
        {
            JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament();
            fetchLeaderboardSignal.Dispatch(joinedTournament.id, false);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LEADERBOARD_VIEW);
        }

        private void OnChestClicked()
        {
            showRewardedAdSignal.Dispatch(AdPlacements.Rewarded_lobby_chest);
        }

        [ListensTo(typeof(RewardedVideoResultSignal))]
        public void OnRewardClaimed(AdsResult result, AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == AdPlacements.Rewarded_lobby_chest)
            {
                if (result == AdsResult.FINISHED)
                {
                    //view.audioService.Play(view.audioService.sounds.SFX_REWARD_UNLOCKED);
                    view.SetupChest();

                    var rewardDlgVO = new RewardDlgV2VO();
                    rewardDlgVO.ShowChest = true;
                    rewardDlgVO.Rewards.Add(new RewardDlgV2VO.Reward(GSBackendKeys.PlayerDetails.GEMS, chestReward));
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REWARD_DLG_V2);
                    updateRewardDlgViewSignal.Dispatch(rewardDlgVO);

                    loadCareerCardSignal.Dispatch();
                }
                else if (result == AdsResult.NOT_AVAILABLE)
                {
                    view.lobbyChestTooltip.SetActive(true);
                }
            }
        }

        [ListensTo(typeof(RewardedVideoAvailableSignal))]
        public void OnRewardedVideoAvailable(AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == AdPlacements.Rewarded_lobby_chest)
            {
                view.isVideoAvailable = true;
                view.SetupChest();
            }
        }

        [ListensTo(typeof(LobbyChestRewardClaimedSignal))]
        public void OnLobbyChestRewardClaimed(int reward)
        {
            if (view.isActiveAndEnabled && reward > 0)
            {
                chestReward = reward;
            }
        }

        [ListensTo(typeof(StartLobbyChampionshipTimerSignal))]
        public void StartLobbyChampionshipTimer()
        {
            if (view.gameObject.activeInHierarchy)
            {
                
                view.SetupChampionshipTimer();
                OnSchedulerSubscription(view.SchedulerCallbackLeaderboard, true);
                //view.StartCoroutine(view.ChampionshipTimer());

                LayoutRebuilder.ForceRebuildLayoutImmediate(view.timerLayout);
            }
        }

        [ListensTo(typeof(LobbySequenceEndedSignal))]
        public void OnLobbyPopupSequenceEnded()
        {
            view.ShowTooltip();
            view.SetupChest();
        }

        private void OnSchedulerSubscription(Action callback,bool subscribe)
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
