/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

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

                if (joinedTournament == null && openTournament == null)
                {
                    // Show tournament end dialogue here, and then fetch Inbox.
                }
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
            if (tournamentId == "" && this.joinedTournament != null)
            {
                tournamentId = this.joinedTournament.id;
            }

            var joinedTournament = tournamentModel.GetJoinedTournament(tournamentId);
            if (joinedTournament != null)
            {
                this.joinedTournament = joinedTournament;
                view.UpdateView(joinedTournament);
            }
            else
            {
                //-- This means that the tournament ended during the match

            }

            this.openTournament = null;
        }

        [ListensTo(typeof(UpdateLiveTournamentRewardsSuccessSignal))]
        public void UpdateLiveTournamentView(string tournamentShortCode)
        {
            var openTournament = tournamentModel.GetOpenTournament(tournamentShortCode);
            if (openTournament != null)
            {
                this.openTournament = openTournament;
                view.UpdateView(openTournament);
            }

            this.joinedTournament = null;
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
                StartTournament();
            }
            else if (view.footer.haveEnoughItems)
            {
                var vo = new VirtualGoodsTransactionVO();
                vo.consumeItemShortCode = view.footer.itemToConsumeShortCode;
                vo.consumeQuantity = 1;
                virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
                virtualGoodsTransactionSignal.Dispatch(vo);
            }
            else if (view.footer.haveEnoughGems)
            {
                var vo = new VirtualGoodsTransactionVO();
                vo.consumeItemShortCode = GSBackendKeys.PlayerDetails.GEMS;
                vo.consumeQuantity = view.ticketStoreItem.currency3Cost;
                virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
                virtualGoodsTransactionSignal.Dispatch(vo);
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

            StartTournament();
        }

        private void StartTournament()
        {
            string tournamentType = joinedTournament != null ? joinedTournament.type : openTournament.type;
            string actionCode;

            switch (tournamentType)
            {
                case TournamentConstants.TournamentType.MIN_1:
                    actionCode = FindMatchAction.ActionCode.Random1.ToString();
                    break;

                case TournamentConstants.TournamentType.MIN_5:
                    actionCode = FindMatchAction.ActionCode.Random.ToString();
                    break;

                case TournamentConstants.TournamentType.MIN_10:
                    actionCode = FindMatchAction.ActionCode.Random10.ToString();
                    break;

                default:
                    actionCode = FindMatchAction.ActionCode.Random.ToString();
                    break;
            }

            tournamentModel.StopScheduledCoroutine();

            tournamentModel.currentMatchTournamentType = tournamentType;
            tournamentModel.currentMatchTournament = joinedTournament;

            FindMatchAction.Random(findMatchSignal, actionCode, joinedTournament != null ? joinedTournament.id : openTournament.shortCode);
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
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
    }
}
