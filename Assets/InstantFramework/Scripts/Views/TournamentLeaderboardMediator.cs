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
        [Inject] public GetProfilePictureSignal getProfilePictureSignal { get; set; }
        [Inject] public LoadSpotInventorySignal loadSpotInventorySignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ITournamentsModel tournamentModel { get; set; }
        [Inject] public INotificationsModel notificationsModel { get; set; }

        //Listeners
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }

        private LiveTournamentData _openTournament = null;
        private JoinedTournamentData _joinedTournament = null;
        private VirtualGoodsTransactionVO transactionVO;
        private bool haveNotEnoughTicketsToPlay = false;

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
            view.loadPictureSignal.AddListener(OnLoadPicture);
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

        [ListensTo(typeof(UpdateTournamentLeaderboardPartialSignal))]
        public void UpdateTournamentViewPartial(string tournamentId)
        {
            view.ClearBars();
            view.DisableFixedPlayerBar();

            var joinedTournament = tournamentModel.GetJoinedTournament(tournamentId);
            if (joinedTournament != null)
            {
                this._openTournament = null;

                view.PopulateHeaderAndFooter(joinedTournament);
                this._joinedTournament = joinedTournament;

                return;
            }

            var openTournament = tournamentModel.GetOpenTournament(tournamentId);
            if (openTournament != null)
            {
                this._joinedTournament = null;

                view.PopulateHeaderAndFooter(openTournament);
                this._openTournament = openTournament;

                return;
            }
        }

        [ListensTo(typeof(UpdateTournamentLeaderboardSignal))]
        public void UpdateJoinedTournamentViewEntries(string tournamentId)
        {
            this._openTournament = null;

            if (tournamentId == "" && this._joinedTournament != null)
            {
                tournamentId = this._joinedTournament.id;
            }

            var joinedTournament = tournamentModel.GetJoinedTournament(tournamentId);
            if (joinedTournament != null)
            {
                this._joinedTournament = joinedTournament;
                view.UpdateView(joinedTournament);

                if (tournamentModel.HasTournamentEnded(joinedTournament) == true && joinedTournament.locked == true)
                {
                    joinedTournament.ended = true;
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }
                else
                {
                    joinedTournament.locked = false;
                }
            }
        }

        [ListensTo(typeof(UpdateLiveTournamentRewardsSuccessSignal))]
        public void UpdateLiveTournamentViewEntries(string tournamentShortCode)
        {
            this._joinedTournament = null;

            var openTournament = tournamentModel.GetOpenTournament(tournamentShortCode);
            if (openTournament != null)
            {
                this._openTournament = openTournament;
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
            if (_openTournament != null)
            {
                if (tournamentModel.HasTournamentEnded(_openTournament) == true)
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }
            }
            else if (_joinedTournament != null)
            {
                if (tournamentModel.HasTournamentEnded(_joinedTournament) == true && _joinedTournament.locked == false)
                {
                    _joinedTournament.ended = true;
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }
            }
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO inventory)
        {
            view.UpdateTickets();
        }

        public void OnEnterButtonClicked()
        {
            view.audioService.PlayStandardClick();

            if (_joinedTournament == null)
            {
                var notification = new Notification();
                notification.title = view.localizationService.Get(LocalizationKey.NOTIFICATION_TOURNAMENT_END_TITLE);
                notification.body = view.localizationService.Get(LocalizationKey.NOTIFICATION_TOURNAMENT_END_BODY);
                notification.timestamp = _openTournament.endTimeUTCSeconds * 1000;
                notification.sender = _openTournament.type;
                notification.showInGame = false;
                notificationsModel.RegisterNotification(notification);

                StartTournament("free");
            }
            else 
            {
                transactionVO = new VirtualGoodsTransactionVO();
                transactionVO.consumeItemShortCode = view.footer.itemToConsumeShortCode;
                transactionVO.consumeQuantity = 1;

                if (view.footer.haveEnoughItems)
                {
                    virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
                    virtualGoodsTransactionSignal.Dispatch(transactionVO);
                }
                else
                {
                    //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
                    haveNotEnoughTicketsToPlay = true;
                    var spotInventoryParams = new LoadSpotInventoryParams();
                    spotInventoryParams.itemShortCode = view.footer.itemToConsumeShortCode;
                    spotInventoryParams.itemToUnclockShortCode = "tournament";
                    loadSpotInventorySignal.Dispatch(spotInventoryParams);
                }
            }
            //else if (view.footer.haveEnoughGems)
            //{
            //    transactionVO = new VirtualGoodsTransactionVO();
            //    transactionVO.consumeItemShortCode = GSBackendKeys.PlayerDetails.GEMS;
            //    transactionVO.consumeQuantity = view.ticketStoreItem.currency3Cost;
            //    virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
            //    virtualGoodsTransactionSignal.Dispatch(transactionVO);
            //}
            
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
            string tournamentType = _joinedTournament != null ? _joinedTournament.type : _openTournament.type;
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
            tournamentModel.currentMatchTournament = _joinedTournament;

            if (_joinedTournament != null)
            {
                _joinedTournament.locked = true;
            }

            if (_openTournament != null)
            {
                _openTournament.joined = true;
            }

            analyticsService.Event(AnalyticsEventId.tournament_start_location, AnalyticsContext.main);
            analyticsService.Event($"{AnalyticsEventId.start_tournament}_{currency}", AnalyticsParameter.context, context);
            FindMatchAction.Random(findMatchSignal, actionCode, _joinedTournament != null ? _joinedTournament.id : _openTournament.shortCode);

            _openTournament = null;
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

        [ListensTo(typeof(UnlockCurrentJoinedTournamentSignal))]
        public void UnlockTournament()
        {
            if (_joinedTournament != null)
            {
                _joinedTournament.locked = false;
                _joinedTournament = null;
            }
        }

        private void OnBackPressed()
        {
            UnlockTournament();
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            audioService.PlayStandardClick();
        }

        private void OnLoadPicture(GetProfilePictureVO vo)
        {
            getProfilePictureSignal.Dispatch(vo);
        }

        [ListensTo(typeof(ProfilePictureLoadedSignal))]
        public void OnPictureLoaded(string playerId, Sprite picture)
        {
            view.UpdatePicture(playerId, picture);
        }

        [ListensTo(typeof(SpotInventoryPurchaseCompletedSignal))]
        public void OnSpotInventoryPurchaseCompleted(string key)
        {
            if (view.isActiveAndEnabled && key.Equals("tournament"))
            {
                view.audioService.Play(view.audioService.sounds.SFX_REWARD_UNLOCKED);
                virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
                virtualGoodsTransactionSignal.Dispatch(transactionVO);
            }
        }
    }
}
