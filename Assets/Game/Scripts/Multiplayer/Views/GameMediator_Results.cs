/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:03:30 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.InstantFramework;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using GameSparks.Core;
using GameAnalyticsSDK;
using System.Collections.Generic;

namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        // Dispatch signal
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public CancelHintSingal cancelHintSignal { get; set; }
        [Inject] public VirtualGoodsTransactionSignal virtualGoodsTransactionSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public LoadArenaSignal loadArenaSignal { get; set; }
        [Inject] public UpdateBottomNavSignal updateBottomNavSignal { get; set; }
        [Inject] public GetTournamentLeaderboardSignal getJoinedTournamentLeaderboardSignal { get; set; }
        [Inject] public UpdateTournamentLeaderboardViewSignal updateTournamentLeaderboardView { get; set; }
        [Inject] public ToggleLeaderboardViewNavButtons toggleLeaderboardViewNavButtons { get; set; }
        [Inject] public LoadSpotInventorySignal loadSpotInventorySignal { get; set; }
        [Inject] public ShowAdSignal showAdSignal { get; set; }

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }

        //Listeners
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }

        private VirtualGoodsTransactionVO rewardDoubleTransactionVO;

        public void OnRegisterResults()
        {
            view.InitResults();
            view.InitAnalysis();
            view.backToLobbySignal.AddListener(OnBackToLobby);
            view.refreshLobbySignal.AddListener(OnRefreshLobby);
            view.resultsDialogClosedSignal.AddListener(OnResultsDialogClosedSignal);
            view.resultsDialogOpenedSignal.AddListener(OnResultsDialogOpenedSignal);
            view.boostRatingSignal.AddListener(OnBoostRating);
            view.notEnoughGemsSignal.AddListener(OnNotEnoughItemsToBoost);
            view.backToArenaSignal.AddListener(OnBackToArenaButtonClicked);
            view.doubleRewardSignal.AddListener(OnDoubleReward);
            view.fullAnalysisButtonClickedSignal.AddListener(OnFullAnallysisButtonClicked);
            view.onAnalysiedMoveSelectedSignal.AddListener(OnAnalysiedMoveSelected);
        }

        public void OnRemoveResults()
        {
            view.CleanupResults();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowResultsView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_RESULTS_DLG)
            {
                if (view.challengeSentDialog.activeSelf)
                {
                    view.HideChallengeSent();
                }
                view.FlashClocks(false);
                view.ShowResultsDialog();
                view.OnParentHideAdBanner();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideResultsView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_RESULTS_DLG)
            {
                view.HideResultsDialog();
            }
        }

        [ListensTo(typeof(UpdateResultDialogSignal))]
        public void OnUpdateResults(ResultsVO vo)
        {
            view.UpdateResultsDialog(vo);
        }

        private void OnBackToLobby()
        {
            cancelHintSignal.Dispatch();
            //loadLobbySignal.Dispatch();
        }

        private void OnResultsDialogClosedSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
        }

        private void OnResultsDialogOpenedSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG);
        }

        private void OnRefreshLobby()
        {
            refreshFriendsSignal.Dispatch();
            refreshCommunitySignal.Dispatch(false);
        }

        private void OnBoostRating(string challengeId)
        {
            var jsonData = new GSRequestData().AddString("rewardType", GSBackendKeys.ClaimReward.TYPE_BOOST_RATING)
                                              .AddString("challengeId", challengeId);
            backendService.ClaimReward(jsonData);
        }

        private void OnDoubleReward(VirtualGoodsTransactionVO vo)
        {
            rewardDoubleTransactionVO = vo;
            virtualGoodsTransactionResultSignal.AddOnce(OnRewardDoubled);
            virtualGoodsTransactionSignal.Dispatch(vo);
        }

        private void OnRewardDoubled(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                view.OnRewardDoubled();
                analyticsService.Event(AnalyticsEventId.gems_used, AnalyticsContext.coin_doubler);
                analyticsService.ResourceEvent(GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, rewardDoubleTransactionVO.consumeQuantity, "booster_used", AnalyticsContext.coin_doubler.ToString());
            }
        }

        [ListensTo(typeof(UpdateEloScoresSignal))]
        public void OnUpdateEloScoresSignal(EloVO vo)
        {
            if (view.IsVisible())
            {
                view.resultsRatingValueLabel.text = vo.playerEloScore.ToString();
            }
        }

        [ListensTo(typeof(RatingBoostedSignal))]
        public void OnRatingBoosted(int ratingBoost, int consumedGems)
        {
            if (view.IsVisible())
            {
                view.OnRatingBoosted(ratingBoost);
                analyticsService.Event(AnalyticsEventId.gems_used, AnalyticsContext.rating_booster);
                analyticsService.ResourceEvent(GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, consumedGems, "booster_used", AnalyticsContext.rating_booster.ToString());
            }
        }

        private void OnNotEnoughItemsToBoost()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
        }

        private void OnBackToArenaButtonClicked()
        {
            OnBackToLobby();
            loadArenaSignal.Dispatch();
            updateBottomNavSignal.Dispatch(BottomNavView.ButtonId.Arena);

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS);
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO inventory)
        {
            if (view.IsVisible())
            {
                view.SetupBoostPrice();
                view.SetupSpecialHintButton();
                view.SetupRewardDoublerPrice();
            }
        }

        private void OnFullAnallysisButtonClicked(List<MoveAnalysis> list)
        {
            OnResultsDialogClosedSignal();
            view.UpdateAnalysisView(list);
        }

        private void OnAnalysiedMoveSelected(List<MoveAnalysis> list)
        {
            
        }
    }
}
