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

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        //Listeners
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }

        private string challengeId;

        public void OnRegisterResults()
        {
            view.InitResults();
            view.backToLobbySignal.AddListener(OnBackToLobby);
            view.refreshLobbySignal.AddListener(OnRefreshLobby);
            view.resultsDialogClosedSignal.AddListener(OnResultsDialogClosedSignal);
            view.resultsDialogOpenedSignal.AddListener(OnResultsDialogOpenedSignal);
            view.boostRatingSignal.AddListener(OnBoostRating);
            view.notEnoughGemsSignal.AddListener(OnNotEnoughItemsToBoost);
            view.playTournamentMatchSignal.AddListener(OnPlayTournamentMatchButtonClicked);
            view.backToArenaSignal.AddListener(OnBackToArenaButtonClicked);
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
            view.UpdateDialogueType(vo.tournamentMatch);
            view.UpdateResultsDialog(vo);
        }

        private void OnBackToLobby()
        {
            cancelHintSignal.Dispatch();
            loadLobbySignal.Dispatch();
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

        private void OnBoostRating(string challengeId, VirtualGoodsTransactionVO vo)
        {
            transactionVO = vo;
            this.challengeId = challengeId;
            virtualGoodsTransactionResultSignal.AddOnce(OnTransactionResult);
            virtualGoodsTransactionSignal.Dispatch(vo);
        }

        private void OnTransactionResult(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                var jsonData = new GSRequestData().AddString("rewardType", GSBackendKeys.ClaimReward.TYPE_BOOST_RATING)
                                                  .AddString("challengeId", challengeId);

                backendService.ClaimReward(jsonData);

                analyticsService.ResourceEvent(GAResourceFlowType.Sink, CollectionsUtil.GetContextFromString(transactionVO.consumeItemShortCode).ToString(), transactionVO.consumeQuantity, "booster_used", "rating_booster");
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

        [ListensTo(typeof(RatingBoostAnimSignal))]
        public void OnRatingBoostAnimation(int ratingBoost)
        {
            if (view.IsVisible())
            {
                view.PlayEloBoostedAnimation(ratingBoost);
            }
        }

        private void OnNotEnoughItemsToBoost()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
        }

        private void OnPlayTournamentMatchButtonClicked()
        {
            view.audioService.PlayStandardClick();

            if (view.haveEnoughItems)
            {
                transactionVO = new VirtualGoodsTransactionVO();
                transactionVO.consumeItemShortCode = view.tournamentMatchResultDialog.ticketsShortCode;
                transactionVO.consumeQuantity = 1;
                virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
                virtualGoodsTransactionSignal.Dispatch(transactionVO);
            }
            else if (view.haveEnoughGems)
            {
                transactionVO = new VirtualGoodsTransactionVO();
                transactionVO.consumeItemShortCode = GSBackendKeys.PlayerDetails.GEMS;
                transactionVO.consumeQuantity = view.ticketStoreItem.currency3Cost;
                virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
                virtualGoodsTransactionSignal.Dispatch(transactionVO);
            }
            else
            {
                SpotPurchaseMediator.customContext = "tournament_end_card";
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
            }            
        }

        private void OnItemConsumed(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                return;
            }

            var currency = CollectionsUtil.GetContextFromString(transactionVO.consumeItemShortCode).ToString();
            analyticsService.ResourceEvent(GAResourceFlowType.Sink, currency, transactionVO.consumeQuantity, "tournament", "end_card");
            StartMatch(currency);
        }

        private void StartMatch(string currency)
        {
            var joinedTournament = tournamentsModel.currentMatchTournament;
            string tournamentType = joinedTournament.type;
            string actionCode;
            string context;

            switch (tournamentType)
            {
                case TournamentConstants.TournamentType.MIN_1:
                    actionCode = FindMatchAction.ActionCode.Random1.ToString();
                    context = "1_min_bullet";
                    break;

                case TournamentConstants.TournamentType.MIN_5:
                    actionCode = FindMatchAction.ActionCode.Random.ToString();
                    context = "5_min_blitz";
                    break;

                case TournamentConstants.TournamentType.MIN_10:
                    actionCode = FindMatchAction.ActionCode.Random10.ToString();
                    context = "10_min_rapid";
                    break;

                default:
                    actionCode = FindMatchAction.ActionCode.Random.ToString();
                    context = "5_min_blitz";
                    break;
            }

            tournamentsModel.currentMatchTournamentType = tournamentType;
            tournamentsModel.currentMatchTournament = joinedTournament;
            joinedTournament.locked = true;

            analyticsService.Event(AnalyticsEventId.tournament_start_location, AnalyticsContext.end_game_card);
            analyticsService.Event($"{AnalyticsEventId.start_tournament}_{currency}", AnalyticsParameter.context, context);

            FindMatchAction.Random(findMatchSignal, actionCode, joinedTournament.id);
        }

        private void OnBackToArenaButtonClicked()
        {
            OnBackToLobby();
            loadArenaSignal.Dispatch();
            updateBottomNavSignal.Dispatch(BottomNavView.ButtonId.Arena);

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS);
            if (view.tournamentEnded)
            {
                updateTournamentLeaderboardView.Dispatch();
                //getJoinedTournamentLeaderboardSignal.Dispatch(tournamentsModel.currentMatchTournament.id, true);
            }
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO inventory)
        {
            if (view.IsVisible())
            {
                view.SetupBoostPrice();
                view.SetupSpecialHintButton();
            }
        }
    }
}
