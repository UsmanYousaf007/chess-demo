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
        [Inject] public ToggleLeaderboardViewNavButtons toggleLeaderboardViewNavButtons { get; set; }
        [Inject] public LoadSpotInventorySignal loadSpotInventorySignal { get; set; }
        [Inject] public ShowAdSignal showAdSignal { get; set; }

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        //Listeners
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }

        private string challengeId;
        private VirtualGoodsTransactionVO ratingBoosterTransactionVO;
        private VirtualGoodsTransactionVO ticketTransactionVO;
        private string spotInventoryPurchaseType;

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
            ratingBoosterTransactionVO = vo;
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

                analyticsService.ResourceEvent(GAResourceFlowType.Sink, CollectionsUtil.GetContextFromString(ratingBoosterTransactionVO.consumeItemShortCode).ToString(), ratingBoosterTransactionVO.consumeQuantity, "booster_used", "rating_booster");
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

        private void OnNotEnoughItemsToBoost(VirtualGoodsTransactionVO vo)
        {
            //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
            ratingBoosterTransactionVO = vo;
            var spotInventoryParams = new LoadSpotInventoryParams();
            spotInventoryParams.itemShortCode = vo.consumeItemShortCode;
            spotInventoryParams.itemToUnclockShortCode = vo.consumeItemShortCode;
            loadSpotInventorySignal.Dispatch(spotInventoryParams);
        }

        private void OnPlayTournamentMatchButtonClicked()
        {
            var joinedTournament = tournamentsModel.currentMatchTournament;
            if (joinedTournament != null && tournamentsModel.HasTournamentEnded(joinedTournament) == true)
            {
                OnBackToArenaButtonClicked();
                return;
            }

            ticketTransactionVO = new VirtualGoodsTransactionVO();
            ticketTransactionVO.consumeItemShortCode = view.tournamentMatchResultDialog.ticketsShortCode;
            ticketTransactionVO.consumeQuantity = 1;

            if (view.haveEnoughItems)
            {
                virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
                virtualGoodsTransactionSignal.Dispatch(ticketTransactionVO);
            }
            //else if (view.haveEnoughGems)
            //{
            //    transactionVO = new VirtualGoodsTransactionVO();
            //    transactionVO.consumeItemShortCode = GSBackendKeys.PlayerDetails.GEMS;
            //    transactionVO.consumeQuantity = view.ticketStoreItem.currency3Cost;
            //    virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
            //    virtualGoodsTransactionSignal.Dispatch(transactionVO);
            //}
            else
            {
                //SpotPurchaseMediator.customContext = "tournament_end_card";
                //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);

                SpotInventoryMediator.customContext = "tournament_end_card";
                var spotInventoryParams = new LoadSpotInventoryParams();
                spotInventoryParams.itemShortCode = ticketTransactionVO.consumeItemShortCode;
                spotInventoryParams.itemToUnclockShortCode = ticketTransactionVO.consumeItemShortCode;
                loadSpotInventorySignal.Dispatch(spotInventoryParams);
                spotInventoryPurchaseType = string.Empty;
            }            
        }

        private void OnItemConsumed(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                return;
            }

            var currency = CollectionsUtil.GetContextFromString(ticketTransactionVO.consumeItemShortCode).ToString();
            analyticsService.ResourceEvent(GAResourceFlowType.Sink, currency, ticketTransactionVO.consumeQuantity, "tournament", "end_card");
            currency = string.IsNullOrEmpty(spotInventoryPurchaseType) ? currency : spotInventoryPurchaseType;

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

            // Analytics
            analyticsService.Event(AnalyticsEventId.tournament_start_location, AnalyticsContext.end_game_card);
            analyticsService.Event($"{AnalyticsEventId.start_tournament}_{currency}", AnalyticsParameter.context, context);

            // Show tournament pre-game ad here.
            var currentTournament = tournamentsModel.currentMatchTournament;
            long tournamentTimeLeftSeconds = tournamentsModel.CalculateTournamentTimeLeftSeconds(currentTournament);
            if (tournamentTimeLeftSeconds < Settings.Ads.TIME_DISABLE_TOURNAMENT_PREGAME_ADS)
            {
                FindMatchAction.Random(findMatchSignal, actionCode, joinedTournament.id);
            }
            else
            {
                playerModel.adContext = AnalyticsContext.interstitial_tournament_pregame;
                ResultAdsVO vo = new ResultAdsVO();
                vo.adsType = AdType.Interstitial;
                vo.actionCode = actionCode;
                vo.tournamentId = joinedTournament.id;
                showAdSignal.Dispatch(vo, false);
            }
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
            }
        }

        [ListensTo(typeof(SpotInventoryPurchaseCompletedSignal))]
        public void OnSpotInventoryPurchaseCompleted(string key, string purchaseType)
        {
            if (view.isActiveAndEnabled)
            {
                if (key.Equals(view.resultsBoostRatingShortCode))
                {
                    view.BoostRating(ratingBoosterTransactionVO);
                }
                else if (key.Equals(view.tournamentMatchResultDialog.ticketsShortCode))
                {
                    spotInventoryPurchaseType = purchaseType;
                    virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
                    virtualGoodsTransactionSignal.Dispatch(ticketTransactionVO);
                }
            }
        }
    }
}
