/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using GameAnalyticsSDK;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;

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
        [Inject] public LoadRewardDlgViewSignal loadRewardDlgViewSignal { get; set; }
        [Inject] public LoadInboxSignal loadInboxSignal { get; set; }
        [Inject] public UpdateBottomNavSignal updateBottomNavSignal { get; set; }
        [Inject] public ShowAdSignal showAdSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ITournamentsModel tournamentModel { get; set; }
        [Inject] public INotificationsModel notificationsModel { get; set; }
        [Inject] public IInboxModel inboxModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }

        //Listeners
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }

        private Signal onRewardDlgClosedSignal = new Signal();

        private LiveTournamentData _openTournament = null;
        private JoinedTournamentData _joinedTournament = null;
        private VirtualGoodsTransactionVO transactionVO;
        private bool haveNotEnoughTicketsToPlay = false;
        private string rewardMessageId = null;
        private bool goBackToArena = false;
        private bool showTournamentOverDialog = false;
        private string spotInventoryPurchaseType;

        public override void OnRegister()
        {
            view.Init();

            // Button click handlers
            view.playerBarClickedSignal.AddListener(OnPlayerBarClicked);
            view.playerBarChestClickSignal.AddListener(OnPlayerBarChestClicked);
            view.footer.enterButtonClickedSignal.AddListener(OnEnterButtonClicked);
            view.footer.resultsContinueButtonClickedSignal.AddListener(OnCollectRewardButtonClicked);
            view.infoBar.rulesButtonClickedSignal.AddListener(OnRulesButtonClicked);
            view.infoBar.totalScoreButtonClickedSignal.AddListener(OnTotalScoreButtonClicked);
            view.infoBar.gameModeButtonClickedSignal.AddListener(OnGameModeButtonClicked);
            view.loadPictureSignal.AddListener(OnLoadPicture);
            view.backSignal.AddListener(OnBackPressed);

            onRewardDlgClosedSignal.AddListener(OnRewardClosed);

        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW)
            {
                view.Show();
                if (showTournamentOverDialog)
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }
                analyticsService.ScreenVisit(AnalyticsScreen.tournament_leaderboard);
            }
            else if (viewId == NavigatorViewId.TOURNAMENT_OVER_DLG)
            {
                showTournamentOverDialog = false;
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW)
            {
                view.Hide();
            }
            else if (viewId == NavigatorViewId.CHEST_INFO_DLG)
            {
                if (IsTournamentOpen() == false)
                {
                    if (_joinedTournament != null && _joinedTournament.ended == true)
                    {
                        return;
                    }

                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }
            }
        }

        [ListensTo(typeof(UpdateTournamentLeaderboardPartialSignal))]
        public void UpdateTournamentViewPartial(string tournamentId)
        {
            view.EnableNavButtons(true);

            goBackToArena = false;

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

                if (joinedTournament.locked == true && tournamentModel.HasTournamentEnded(joinedTournament) == true && joinedTournament.ended == false)
                {
                    joinedTournament.ended = true;
                    showTournamentOverDialog = true;
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }
                else
                {
                    joinedTournament.locked = false;
                }

                view.backButton.transform.parent.gameObject.SetActive(!joinedTournament.ended);

                view.UpdateView(joinedTournament);
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

                if (tournamentModel.HasTournamentEnded(openTournament) == true)
                {
                    showTournamentOverDialog = true;
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }

                view.backButton.transform.parent.gameObject.SetActive(true);

                view.UpdateView(openTournament);
            }
        }

        [ListensTo(typeof(ToggleLeaderboardViewNavButtons))]
        public void OnToggleLeaderboardViewNavButtons(bool enable)
        {
            view.EnableNavButtons(enable);
        }

        [ListensTo(typeof(UpdateTournamentLeaderboardViewSignal))]
        public void UpdateLiveTournamentView()
        {
            if (_openTournament != null)
            {
                if (tournamentModel.HasTournamentEnded(_openTournament) == true)
                {
                    showTournamentOverDialog = true;
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }

                view.backButton.transform.parent.gameObject.SetActive(true);

                view.UpdateView(_openTournament);
                _joinedTournament = null;
            }
            else if (_joinedTournament != null)
            {
                if (tournamentModel.HasTournamentEnded(_joinedTournament) == true && _joinedTournament.locked == false && _joinedTournament.ended == false)
                {
                    _joinedTournament.ended = true;
                    showTournamentOverDialog = true;
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }

                view.backButton.transform.parent.gameObject.SetActive(!_joinedTournament.ended);

                view.UpdateView(_joinedTournament);
                _openTournament = null;
            }
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO inventory)
        {
            view.UpdateTickets();
        }

        [ListensTo(typeof(OnTournamentEndRewardViewClickedSignal))]
        public void OnTournamentRewardViewClicked(string messageId)
        {
            rewardMessageId = messageId;
        }

        [ListensTo(typeof(TournamentOverDialogueClosedSignal))]
        public void OnTournamentOverDialogueClosed()
        {
            if (_openTournament != null)
            {
                UnlockTournament();
                navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            }
        }

        public void OnEnterButtonClicked()
        {
            view.audioService.PlayStandardClick();

            if (_joinedTournament == null)
            {
                notificationsModel.UnregisterNotifications(_openTournament.type);
                var notification = new Notification();
                notification.title = view.localizationService.Get(LocalizationKey.NOTIFICATION_TOURNAMENT_END_TITLE);
                notification.body = view.localizationService.Get(LocalizationKey.NOTIFICATION_TOURNAMENT_END_BODY);
                notification.timestamp = _openTournament.endTimeUTCSeconds * 1000;
                notification.sender = _openTournament.type;
                notification.showInGame = false;
                notificationsModel.RegisterNotification(notification);

                var context = navigatorModel.previousState.GetType() == typeof(NSLobby) ? AnalyticsContext.lobby : AnalyticsContext.tournaments_tab;
                analyticsService.Event(AnalyticsEventId.tournament_first_game_start_location, context);
                StartTournament("free");
            }
            else
            {
                transactionVO = new VirtualGoodsTransactionVO();
                transactionVO.consumeItemShortCode = view.footer.itemToConsumeShortCode;
                transactionVO.consumeQuantity = 1;

                if (view.footer.haveEnoughItems)
                {
                    if (IsTournamentOpen())
                    {
                        virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
                        virtualGoodsTransactionSignal.Dispatch(transactionVO);
                    }
                }
                else
                {
                    //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
                    haveNotEnoughTicketsToPlay = true;
                    var spotInventoryParams = new LoadSpotInventoryParams();
                    spotInventoryParams.itemShortCode = view.footer.itemToConsumeShortCode;
                    spotInventoryParams.itemToUnclockShortCode = "tournament";
                    loadSpotInventorySignal.Dispatch(spotInventoryParams);
                    spotInventoryPurchaseType = string.Empty;
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

        private void OnCollectRewardButtonClicked()
        {
            view.audioService.PlayStandardClick();

            if (rewardMessageId != null)
            {
                goBackToArena = false;
                loadRewardDlgViewSignal.Dispatch(rewardMessageId, onRewardDlgClosedSignal);
            }
            else
            {
                goBackToArena = true;

                rewardMessageId = inboxModel.GetTournamentRewardMessage(_joinedTournament.id)?.id;
                if (rewardMessageId != null)
                {
                    loadRewardDlgViewSignal.Dispatch(rewardMessageId, onRewardDlgClosedSignal);
                }
            }
        }

        private void OnRewardClosed()
        {
            loadInboxSignal.Dispatch();
            UnlockTournament();

            if (goBackToArena)
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
                audioService.PlayStandardClick();
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_ARENA);
                updateBottomNavSignal.Dispatch(BottomNavView.ButtonId.Arena);
            }
            else
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
                audioService.PlayStandardClick();
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_INBOX);
            }

            rewardMessageId = null;
            goBackToArena = false;
            showTournamentOverDialog = false;
        }

        private void OnItemConsumed(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                return;
            }

            if (IsTournamentOpen())
            {
                var currency = CollectionsUtil.GetContextFromString(transactionVO.consumeItemShortCode).ToString();
                analyticsService.ResourceEvent(GAResourceFlowType.Sink, currency, transactionVO.consumeQuantity, "tournament", "main");
                preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_USED][transactionVO.consumeItemShortCode] += transactionVO.consumeQuantity;
                currency = string.IsNullOrEmpty(spotInventoryPurchaseType) ? currency : spotInventoryPurchaseType;
                StartTournament(currency);
            }
            else
            {
                showTournamentOverDialog = true;
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
            }
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


            view.EnableNavButtons(false);

            if (_openTournament != null)
            {
                _openTournament.joined = true;
            }

            // Analytics
            analyticsService.Event(AnalyticsEventId.tournament_start_location, AnalyticsContext.main);
            analyticsService.Event($"{AnalyticsEventId.start_tournament}_{currency}", AnalyticsParameter.context, context);

            // Show tournament match pre-game ad here. Skip if 10 mins are left in tournament to end
            long timeLeftSeconds = CalculateTournamentTimeLeftSeconds();
            string tournamentId = _joinedTournament != null ? _joinedTournament.id : _openTournament.shortCode;
            if (adsSettingsModel.showPregameTournament == false || timeLeftSeconds < adsSettingsModel.secondsLeftDisableTournamentPregame)
            {
                FindMatchAction.Random(findMatchSignal, actionCode, tournamentId);
            }
            else
            {
                playerModel.adContext = AnalyticsContext.interstitial_tournament_pregame;
                ResultAdsVO vo = new ResultAdsVO();
                vo.adsType = AdType.Interstitial;
                vo.actionCode = actionCode;
                vo.tournamentId = tournamentId;
                vo.placementId = AdPlacements.Interstitial_tournament_pre;
                showAdSignal.Dispatch(vo, false);
            }
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

            _openTournament = null;
        }

        private void OnBackPressed()
        {
            showTournamentOverDialog = false;
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
            if (view.isActiveAndEnabled)
            {
                view.UpdatePicture(playerId, picture);
            }
        }

        [ListensTo(typeof(SpotInventoryPurchaseCompletedSignal))]
        public void OnSpotInventoryPurchaseCompleted(string key, string purchaseType)
        {
            if (view.isActiveAndEnabled && key.Equals("tournament"))
            {
                if (IsTournamentOpen())
                {
                    view.audioService.Play(view.audioService.sounds.SFX_REWARD_UNLOCKED);
                    spotInventoryPurchaseType = purchaseType;
                    virtualGoodsTransactionResultSignal.AddOnce(OnItemConsumed);
                    virtualGoodsTransactionSignal.Dispatch(transactionVO);
                }
                else
                {
                    showTournamentOverDialog = true;
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_OVER_DLG);
                }
            }
        }

        private bool IsTournamentOpen()
        {
            if (_openTournament != null)
            {
                if (tournamentModel.HasTournamentEnded(_openTournament) == true)
                {
                    return false;
                }
            }

            if (_joinedTournament != null)
            {
                if (tournamentModel.HasTournamentEnded(_joinedTournament) == true)
                {
                    return false;
                }
            }

            return true;
        }

        private long CalculateTournamentTimeLeftSeconds()
        {
            if (_openTournament != null)
            {
                return tournamentModel.CalculateTournamentTimeLeftSeconds(_openTournament);
            }
            else if (_joinedTournament != null)
            {
                return tournamentModel.CalculateTournamentTimeLeftSeconds(_joinedTournament);
            }

            return 0;
        }
    }
}
