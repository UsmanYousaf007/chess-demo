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

using System.Collections.Generic;
using GameAnalyticsSDK;
using GameSparks.Core;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;

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
        [Inject] public RenderMoveAnalysisSignal renderMoveAnalysisSignal { get; set; }
        [Inject] public GetFullAnalysisSignal getFullAnalysisSignal { get; set; }
        [Inject] public UpdateGetGameAnalysisDlgSignal updateGetGameAnalysisDlg { get; set; }
        [Inject] public ShowRewardedAdSignal showRewardedAdSignal { get; set; }

        //Services
        [Inject] public ISchedulerService schedulerService { get; set; }

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
            view.serverClock = backendService.serverClock;
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
            view.showGetFullAnalysisDlg.AddListener(OnShowGetGameAnalysisDlg);
            view.ratingBoosterRewardSignal.AddListener(OnPlayRewardedVideoClicked);
            view.schedulerSubscription.AddListener(OnSchedulerSubscriptionToggle);
            view.showGameAnalysisSignal.AddListener(OnShowGameAnalysisSignal);
            view.showAnalyzingSignal.AddListener(OnShowAnalyzing);
        }

        public void OnRemoveResults()
        {
            view.CleanupResults();
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
            backendService.ClaimReward(jsonData).Then(OnClaimRewardStatus);
        }

        private void OnClaimRewardStatus(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                view.SetupRatingBoostButtonsSection(true);
            }
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
                if (consumedGems > 0)
                {
                    analyticsService.Event(AnalyticsEventId.gems_used, AnalyticsContext.rating_booster);
                    analyticsService.ResourceEvent(GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, consumedGems, "booster_used", AnalyticsContext.rating_booster.ToString());
                }

                else
                {
                    analyticsService.Event(AnalyticsEventId.rv_used, AnalyticsContext.rating_booster);
                }

                view.OnRewardClaimed();
                view.OnRatingBoosted(ratingBoost);               
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
                view.SetupFullAnalysisPrice();
            }
        }

        private void OnFullAnallysisButtonClicked()
        {
            getFullAnalysisSignal.Dispatch();
        }

        private void OnAnalysiedMoveSelected(List<MoveAnalysis> list)
        {
            renderMoveAnalysisSignal.Dispatch(list);
        }

        private void OnShowGameAnalysisSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_GAME_ANALYSIS);
        }

        [ListensTo(typeof(FullAnalysisBoughtSignal))]
        public void OnFullAnalysisBought()
        {
            if (view.IsVisible())
            {
                view.ShowAnalysis();
            }
        }

        private void OnShowGetGameAnalysisDlg(MatchAnalysis matchAnalysis, StoreItem storeItem, bool availableForFree)
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_GAME_BUY_ANALYSIS_DLG);
            updateGetGameAnalysisDlg.Dispatch(matchAnalysis, storeItem, availableForFree);
        }

        private void OnPlayRewardedVideoClicked()
        {
            showRewardedAdSignal.Dispatch(AdPlacements.RV_rating_booster);
        }

        private void OnShowAnalyzing()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_GAME_ANALYZING_DLG);
        }

        private void OnSchedulerSubscriptionToggle(bool subscribe)
        {
            if (subscribe)
            {
                schedulerService.Subscribe(view.SchedulerCallBack);
            }
            else
            {
                schedulerService.UnSubscribe(view.SchedulerCallBack);
            }
        }

        [ListensTo(typeof(RewardedVideoResultSignal))]
        public void OnRewardedVideoResult(AdsResult result, AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == AdPlacements.RV_rating_booster)
            {
                if (result == AdsResult.NOT_AVAILABLE || result == AdsResult.FAILED)
                {
                    view.EnableVideoAvailabilityTooltip();
                }
            }
        }

        [ListensTo(typeof(MoveAnalysiedSignal))]
        public void OnMoveAnalysiedSignal(string challengeId, MoveAnalysis moveAnalysis, MatchAnalysis matchAnalysis)
        {
            view.MoveAnalysied(challengeId, moveAnalysis, matchAnalysis);
        }
    }
}
