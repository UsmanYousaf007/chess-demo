/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System.Collections;

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

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.leaderboardButtonClickedSignal.AddListener(OnLeaderboardClicked);
            view.chestButtonClickedSignal.AddListener(OnChestClicked);

        }

        [ListensTo(typeof(UpdateProfileSignal))]
        public void OnUpdateProfile(ProfileVO vo)
        {
            view.UpdateView(vo);
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
            //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CHAMPIONSHIP_NEW_RANK_DLG);
        }

        private void OnChestClicked()
        {
            showRewardedAdSignal.Dispatch(AdPlacements.Rewarded_lobby_chest);
        }

        [ListensTo(typeof(RewardedVideoResultSignal))]
        public void OnRewardClaimed(AdsResult result, AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == AdPlacements.Rewarded_lobby_chest && result == AdsResult.FINISHED)
            {
                view.audioService.Play(view.audioService.sounds.SFX_REWARD_UNLOCKED);
                view.SetupChest();

                var rewardDlgVO = new RewardDlgV2VO();
                rewardDlgVO.Rewards.Add(new RewardDlgV2VO.Reward(GSBackendKeys.PlayerDetails.COINS, 2000));
                updateRewardDlgViewSignal.Dispatch(rewardDlgVO);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REWARD_DLG_V2);
            }
        }

        [ListensTo(typeof(RewardedVideoAvailableSignal))]
        public void OnRewardedVideoAvailable(AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == AdPlacements.Rewarded_lobby_chest)
            {
                view.SetupChest();
            }
        }
    }
}
