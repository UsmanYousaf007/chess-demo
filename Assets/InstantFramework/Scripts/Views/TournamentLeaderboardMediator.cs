/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using GameAnalyticsSDK;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class TournamentLeaderboardMediator : Mediator
    {
        // View injection
        [Inject] public TournamentLeaderboardView view { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public UpdateChestInfoDlgViewSignal updateChestInfoDlgViewSignal { get; set; }
        [Inject] public VirtualGoodsTransactionSignal virtualGoodsTransactionSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ITournamentsModel tournamentModel { get; set; }

        //Listeners
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }

        private LiveTournamentData openTournament = null;
        private JoinedTournamentData joinedTournament = null;
        private VirtualGoodsTransactionVO transactionVO;

        public override void OnRegister()
        {
            view.Init();

            // Button click handlers
            view.playerBarClickedSignal.AddListener(OnPlayerBarClicked);
            view.playerBarChestClickSignal.AddListener(OnPlayerBarChestClicked);
            view.footer.enterButtonClickedSignal.AddListener(OnEnterButtonClicked);
            view.infoBar.rulesButtonClickedSignal.AddListener(OnRulesButtonClicked);
            view.infoBar.totalScoreButtonClickedSignal.AddListener(OnTotalScoreButtonClicked);
            view.infoBar.gameModeButtonClickedSignal.AddListener(OnGameModeButtonClicked);
            view.backSignal.AddListener(OnBackPressed);

        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.tournament_leaderboard);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateTournamentLeaderboardSignal))]
        public void UpdateJoinedTournamentView(string tournamentId)
        {
            this.openTournament = null;

            if (tournamentId == "" && this.joinedTournament != null)
            {
                tournamentId = this.joinedTournament.id;
            }

            var joinedTournament = tournamentModel.GetJoinedTournament(tournamentId);
            if (joinedTournament != null)
            {
                this.joinedTournament = joinedTournament;
                view.UpdateView(joinedTournament);

                if (tournamentModel.HasTournamentEnded(joinedTournament) == true)
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }
            }
        }

        [ListensTo(typeof(UpdateLiveTournamentRewardsSuccessSignal))]
        public void UpdateLiveTournamentView(string tournamentShortCode)
        {
            this.joinedTournament = null;

            var openTournament = tournamentModel.GetOpenTournament(tournamentShortCode);
            if (openTournament != null)
            {
                this.openTournament = openTournament;
                view.UpdateView(openTournament);

                if (tournamentModel.HasTournamentEnded(openTournament) == true)
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }
            }
        }

        [ListensTo(typeof(UpdateTournamentLeaderboardViewSignal))]
        public void UpdateLiveTournamentView()
        {
            if (openTournament != null)
            {
                if (tournamentModel.HasTournamentEnded(openTournament) == true)
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }
            }
            else if (joinedTournament != null)
            {
                if (tournamentModel.HasTournamentEnded(joinedTournament) == true)
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }
            }
        }

        [ListensTo(typeof(TournamentOverDialogClosedSignal))]
        public void OnTournamentOverDialogClosed()
        {
            OnBackPressed();
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO inventory)
        {
            view.PopulateFooter();
        }

        public void OnEnterButtonClicked()
        {
            TLUtils.LogUtil.Log("TournamentLeaderboardMediator::OnEnterButtonClicked()");
            view.audioService.PlayStandardClick();

            if (joinedTournament == null)
            {
                StartTournament("free");
            }
            else if (view.footer.haveEnoughItems)
            {
                transactionVO = new VirtualGoodsTransactionVO();
                transactionVO.consumeItemShortCode = view.footer.itemToConsumeShortCode;
                transactionVO.consumeQuantity = 1;
                virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
                virtualGoodsTransactionSignal.Dispatch(transactionVO);
            }
            else if (view.footer.haveEnoughGems)
            {
                transactionVO = new VirtualGoodsTransactionVO();
                transactionVO.consumeItemShortCode = GSBackendKeys.PlayerDetails.GEMS;
                transactionVO.consumeQuantity = view.ticketStoreItem.currency3Cost;
                virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
                virtualGoodsTransactionSignal.Dispatch(transactionVO);
            }
            else
            {
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
            analyticsService.ResourceEvent(GAResourceFlowType.Sink, currency, transactionVO.consumeQuantity, "tournament", "main");
            StartTournament(currency);
        }

        private void StartTournament(string currency)
        {
            string tournamentType = joinedTournament != null ? joinedTournament.type : openTournament.type;
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

            tournamentModel.currentMatchTournamentType = tournamentType;
            tournamentModel.currentMatchTournament = joinedTournament;

            if (joinedTournament != null)
            {
                joinedTournament.locked = true;
            }

            if (openTournament != null)
            {
                openTournament.joined = true;
            }

            analyticsService.Event(AnalyticsEventId.tournament_start_location, AnalyticsContext.main);
            analyticsService.Event($"{AnalyticsEventId.start_tournament}_{currency}", AnalyticsParameter.context, context);
            FindMatchAction.Random(findMatchSignal, actionCode, joinedTournament != null ? joinedTournament.id : openTournament.shortCode);

            openTournament = null;
        }

        public void OnPlayerBarClicked(TournamentLeaderboardPlayerBar playerBar)
        {
            TLUtils.LogUtil.Log("TournamentLeaderboardMediator::OnPlayerBarClicked()");
        }

        public void OnPlayerBarChestClicked(TournamentReward reward)
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CHEST_INFO_DLG);
            updateChestInfoDlgViewSignal.Dispatch(reward);
        }

        public void OnRulesButtonClicked()
        {
            TLUtils.LogUtil.Log("TournamentLeaderboardMediator::OnRulesButtonClicked()");
        }

        public void OnTotalScoreButtonClicked()
        {
            TLUtils.LogUtil.Log("TournamentLeaderboardMediator::OnTotalScoreButtonClicked()");
        }

        public void OnGameModeButtonClicked()
        {
            TLUtils.LogUtil.Log("TournamentLeaderboardMediator::OnGameModeButtonClicked()");
        }

        private void OnBackPressed()
        {
            if (joinedTournament != null)
            {
                joinedTournament.locked = false;
                joinedTournament = null;
            }

            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
    }
}
