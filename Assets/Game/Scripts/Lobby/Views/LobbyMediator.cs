/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using TurboLabz.InstantGame;
using TurboLabz.CPU;

namespace TurboLabz.InstantFramework
{
    public class LobbyMediator : Mediator
    {
        // View injection
        [Inject] public LobbyView view { get; set; }

        // Dispatch signals
        [Inject] public AdjustStrengthSignal adjustStrengthSignal { get; set; }
        [Inject] public AuthFaceBookSignal authFacebookSignal { get; set; }
        [Inject] public LoadFriendsSignal loadFriendsSignal { get; set; }
        [Inject] public ShowProfileDialogSignal showProfileDialogSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public ShareAppSignal shareAppSignal { get; set; }
        [Inject] public TapLongMatchSignal tapLongMatchSignal { get; set; }
        [Inject] public SetActionCountSignal setActionCountSignal { get; set; }
        [Inject] public AcceptSignal acceptSignal { get; set; }
        [Inject] public DeclineSignal declineSignal { get; set; }
        [Inject] public CloseStripSignal closeStripSignal { get; set; }
        [Inject] public TurboLabz.Multiplayer.ResignSignal resignSignal { get; set; }
        [Inject] public RemoveCommunityFriendSignal removeCommunityFriendSignal { get; set; }
        [Inject] public LoadStatsSignal loadStatsSignal { get; set; }
        [Inject] public StartCPUGameSignal startCPUGameSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public LoadChatSignal loadChatSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ShowAdSignal showAdSignal { get; set; }
        [Inject] public LoadTopicsViewSignal loadTopicsViewSignal { get; set; }
        [Inject] public GetTournamentLeaderboardSignal getJoinedTournamentLeaderboardSignal { get; set; }
        [Inject] public FetchLiveTournamentRewardsSignal fetchLiveTournamentRewardsSignal { get; set; }
        [Inject] public LoadArenaSignal loadArenaSignal { get; set; }
        [Inject] public UpdateBottomNavSignal updateBottomNavSignal { get; set; }
        [Inject] public UpdateTournamentLeaderboardPartialSignal updateTournamentLeaderboardPartialSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IPromotionsService promotionsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.playMultiplayerButtonClickedSignal.AddListener(OnQuickMatchBtnClicked);
            view.playMultiplayerClassicButtonClickedSignal.AddListener(OnClassicMatchBtnClicked);
            view.playCPUButtonClickedSignal.AddListener(OnPlayComputerMatchBtnClicked);
            view.upgradeToPremiumButtonClickedSignal.AddListener(OnUpgradeToPremiumClicked);
            view.OnLessonsBtnClicked.AddListener(OnLessonsBtnClicked);

            view.facebookButtonClickedSignal.AddListener(OnFacebookButtonClicked);
            view.reloadFriendsSignal.AddOnce(OnReloadFriends);
            view.showProfileDialogSignal.AddListener(OnShowProfileDialog);
            view.refreshCommunityButton.onClick.AddListener(OnRefreshCommunity);
            //view.defaultInviteFriendsButton.onClick.AddListener(OnShareApp);
            view.playButtonClickedSignal.AddListener(OnPlayButtonClicked);
            view.quickMatchFriendButtonClickedSignal.AddListener(OnQuickMatchFriendButtonClicked);
            view.actionCountUpdatedSignal.AddListener(OnActionCountUpdated);
            view.acceptButtonClickedSignal.AddListener(OnAcceptButtonClicked);
            view.declineButtonClickedSignal.AddListener(OnDeclineButtonClicked);
            view.cancelButtonClickedSignal.AddListener(OnCancelButtonClicked);
            view.okButtonClickedSignal.AddListener(OnOkButtonClicked);
            view.removeCommunityFriendSignal.AddListener(OnRemoveCommunityFriend);
            view.decStrengthButtonClickedSignal.AddListener(OnDecStrengthButtonClicked);
            view.incStrengthButtonClickedSignal.AddListener(OnIncStrengthButtonClicked);
            view.showChatSignal.AddListener(OnShowChat);
            view.joinedTournamentButtonClickedSignal.AddListener(OnJoinedTournamentClicked);
            view.openTournamentButtonClickedSignal.AddListener(OnOpenTournamentClicked);
        }

        private void OnDecStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(false);
        }

        private void OnIncStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(true);
        }

        [ListensTo(typeof(UpdateStrengthSignal))]
        public void OnUpdateStrength(LobbyVO vo)
        {
            view.UpdateStrength(vo);
        }

        [ListensTo(typeof(UpdateMenuViewSignal))]
        public void OnUpdateView(LobbyVO vo)
        {
            view.UpdateView(vo);
        }

        //[ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.lobby, facebookService.isLoggedIn());
            }
            else if (viewId == NavigatorViewId.CREATE_MATCH_LIMIT_REACHED_DIALOG)
            {
                view.ShowCreateMatchLimitReacDlg();
            }
            else if (viewId == NavigatorViewId.AD_SKIPPED_DLG)
            {
                view.ShowAdSkippedDailogue(true);
            }
        }

        //[ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY)
            {
                view.Hide();
            }
            else if (viewId == NavigatorViewId.CREATE_MATCH_LIMIT_REACHED_DIALOG)
            {
                view.HideCreateMatchLimitDlg();
            }
            else if (viewId == NavigatorViewId.START_GAME_DLG)
            {
                view.HideStartGameDlg();
            }
            else if (viewId == NavigatorViewId.REMOVE_FRIEND_DLG)
            {
                view.HideRemoveCommunityFriendDlg();
            }
            else if (viewId == NavigatorViewId.START_CPU_GAME_DLG)
            {
                view.HideChooseCPUGameDlg();
            }
            else if (viewId == NavigatorViewId.AD_SKIPPED_DLG)
            {
                view.ShowAdSkippedDailogue(false);
            }
        }

        [ListensTo(typeof(AddFriendsSignal))]
        public void OnUpdateFriends(Dictionary<string, Friend> friends, FriendCategory friendCategory)
        {
            bool isCommunity = false;
            bool isSearched = false;
            if (friendCategory == FriendCategory.COMMUNITY)
            {
                isCommunity = true;
            }
            else if (friendCategory == FriendCategory.SEARCHED)
            {
                isCommunity = true;
                isSearched = true;
                return;
            }

            view.AddFriends(friends, isCommunity, isSearched);
        }

        [ListensTo(typeof(NewFriendAddedSignal))]
        public void OnNewFriendAdded(string friendId)
        {
            view.NewFriendAdded(friendId);
        }

        [ListensTo(typeof(UpdateFriendPicSignal))]
        public void OnUpdateFriendPic(string playerId, Sprite sprite)
        {
            view.UpdateFriendPic(playerId, sprite);
        }

        [ListensTo(typeof(UpdateEloScoresSignal))]
        public void OnUpdateEloScoresSignal(EloVO vo)
        {
            view.UpdateEloScores(vo);
        }

        [ListensTo(typeof(UpdateFriendBarStatusSignal))]
        public void OnUpdateFriendBarStatus(LongPlayStatusVO vo)
        {
            view.UpdateFriendBarStatus(vo);
        }

        [ListensTo(typeof(UpdateFriendOnlineStatusSignal))]
        public void OnUpdateFriendOnlineStatusSignal(ProfileVO vo)
        {
            view.UpdateFriendOnlineStatusSignal(vo);
            view.UpdateStartGameConfirmationDlg(vo);
        }

        [ListensTo(typeof(FriendBarBusySignal))]
        public void OnFriendBarBusy(string playerId, bool busy, CreateLongMatchAbortReason reason)
        {
            view.UpdateFriendBarBusy(playerId, busy, reason);
        }

        [ListensTo(typeof(SortFriendsSignal))]
        public void OnSortFriends()
        {

            view.SortFriends();
        }

        [ListensTo(typeof(SortCommunitySignal))]
        public void OnSortCommunity()
        {
            view.SortCommunity();
        }

        [ListensTo(typeof(ClearCommunitySignal))]
        public void OnClearCommunity()
        {
            view.ClearCommunity();
        }

        [ListensTo(typeof(ClearFriendsSignal))]
        public void OnClearFriends()
        {
            view.ClearFriends();
        }

        [ListensTo(typeof(ClearFriendSignal))]
        public void OnClearFriend(string friendId)
        {
            view.ClearFriend(friendId);
        }

        [ListensTo(typeof(AddUnreadMessagesToBarSignal))]
        public void OnAddUnreadMessages(string friendId, int messagesCount)
        {
            view.AddUnreadMessages(friendId, messagesCount);
        }

        [ListensTo(typeof(ClearUnreadMessagesFromBarSignal))]
        public void OnClearUnreadMessages(string friendId)
        {
            view.ClearUnreadMessages(friendId);
        }

        [ListensTo(typeof(PlayerProfilePicTappedSignal))]
        public void OnPlayerProfileButtonTapped()
        {
            if (gameObject.activeSelf)
            {
                loadStatsSignal.Dispatch();
            }
        }

        [ListensTo(typeof(ChessboardBlockerEnableSignal))]
        public void OnUIBlockerEnable(bool enable)
        {
            if (gameObject.activeSelf)
            {
                view.uiBlocker.SetActive(enable);
            }
        }

        [ListensTo(typeof(SkinUpdatedSignal))]
        public void OnSkinUpdated()
        {
            view.UpdateBarsSkin();
        }

        private void OnFacebookButtonClicked()
        {
            authFacebookSignal.Dispatch();
        }

        private void OnReloadFriends()
        {
            loadFriendsSignal.Dispatch();
        }

        private void OnShowProfileDialog(string playerId, FriendBar bar)
        {
            showProfileDialogSignal.Dispatch(playerId, bar);
        }

        private void OnShowChat(string playerId)
        {
            loadChatSignal.Dispatch(playerId, false);
        }

        private void OnRefreshCommunity()
        {
            refreshCommunitySignal.Dispatch(true);

            // Analytics
            analyticsService.Event(AnalyticsEventId.refresh_community);
        }

        private void OnShareApp()
        {
            shareAppSignal.Dispatch();
        }

        private void OnPlayButtonClicked(string playerId, bool isRanked)
        {
            if (!playerModel.HasSubscription())
            {
                playerModel.adContext = AnalyticsContext.interstitial_pregame;
                ResultAdsVO vo = new ResultAdsVO();
                vo.adsType = AdType.Interstitial;
                vo.isRanked = isRanked;
                vo.friendId = playerId;
                vo.actionCode = "ChallengeClassic";
                vo.placementId = AdPlacements.Interstitial_pregame;

                showAdSignal.Dispatch(vo, false);
                return;
            }
            tapLongMatchSignal.Dispatch(playerId, isRanked);
        }

        private void OnQuickMatchFriendButtonClicked(string playerId, bool isRanked, string actionCode)
        {
            //-- Show UI blocker and spinner here. We are disabling it in the FindMatchCommand's HandleFindMatchErrors method.
            view.ShowProcessing(true, true);

            var friend = playerModel.GetFriend(playerId);

            if (!playerModel.HasSubscription())
            {
                playerModel.adContext = AnalyticsContext.interstitial_pregame;
                ResultAdsVO vo = new ResultAdsVO();
                vo.adsType = AdType.Interstitial;
                vo.actionCode = actionCode;
                vo.friendId = playerId;
                vo.isRanked = isRanked;
                vo.placementId = AdPlacements.Interstitial_pregame;
                
                showAdSignal.Dispatch(vo, false);
                return;
            }

            FindMatchAction.Challenge(findMatchSignal, isRanked, playerId, actionCode);
        }

        private void OnAcceptButtonClicked(string playerId)
        {
            acceptSignal.Dispatch(playerId);
        }

        private void OnDeclineButtonClicked(string playerId)
        {
            declineSignal.Dispatch(playerId);
        }

        private void OnCancelButtonClicked(string playerId)
        {
            resignSignal.Dispatch(playerId);
        }

        private void OnOkButtonClicked(string playerId)
        {
            closeStripSignal.Dispatch(playerId);
        }

        private void OnActionCountUpdated(int count)
        {
            setActionCountSignal.Dispatch(count);
        }

        private void OnRemoveCommunityFriend(string opponentId)
        {
            removeCommunityFriendSignal.Dispatch(opponentId);
        }

        private void OnPlayComputerMatchBtnClicked()
        {
            if (!playerModel.HasSubscription())
            {
                playerModel.adContext = AnalyticsContext.interstitial_pregame;
                ResultAdsVO vo = new ResultAdsVO();
                vo.adsType = AdType.Interstitial;
                vo.placementId = AdPlacements.Interstitial_pregame;
                
                showAdSignal.Dispatch(vo, false);
                return;
            }
            startCPUGameSignal.Dispatch(false);
        }

        private void OnQuickMatchBtnClicked(string actionCode)
        {
            //-- Show UI blocker and spinner here. We are disabling it in the FindMatchCommand's HandleFindMatchErrors method.
            view.ShowProcessing(true, true);

            if (!playerModel.isPremium)
            {
                playerModel.adContext = AnalyticsContext.interstitial_pregame;
                ResultAdsVO vo = new ResultAdsVO();
                vo.adsType = AdType.Interstitial;
                vo.actionCode = actionCode;
                vo.placementId = AdPlacements.Interstitial_pregame;
                
                showAdSignal.Dispatch(vo, false);
                return;
            }

            //FindMatchAction.Random(findMatchSignal, actionCode);
        }

        private void OnClassicMatchBtnClicked()
        {
            //-- Show UI blocker and spinner here. We are disabling it in the FindMatchCommand's HandleFindMatchErrors method.
            view.ShowProcessing(true, true);

            if (!playerModel.HasSubscription())
            {
                playerModel.adContext = AnalyticsContext.interstitial_pregame;
                ResultAdsVO vo = new ResultAdsVO();
                vo.adsType = AdType.Interstitial;
                vo.placementId = AdPlacements.Interstitial_pregame;
                vo.actionCode = FindMatchAction.ActionCode.Random30.ToString();
                showAdSignal.Dispatch(vo, false);
                return;
            }

            //analyticsService.Event("classic_" + AnalyticsEventId.match_find_random, AnalyticsContext.start_attempt);
            //FindMatchAction.Random(findMatchSignal, FindMatchAction.ActionCode.Random30.ToString());
        }

        private void OnLessonsBtnClicked()
        {
            loadTopicsViewSignal.Dispatch();
        }

        [ListensTo(typeof(ShowPromotionSignal))]
        public void OnShowPromotion(PromotionVO vo)
        {
            view.ShowPromotion(vo);
        }

        [ListensTo(typeof(ShowCoachTrainingDailogueSignal))]
        public void OnShowCoachTrainingDailogue()
        {
            view.ShowCoachTrainingDailogue();
        }

        [ListensTo(typeof(ShowStrengthTrainingDailogueSignal))]
        public void OnShowStrengthTrainingDailogue()
        {
            view.ShowStrengthTrainingDailogue();
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnRemoveLobbyPromotion(StoreItem item)
        {
            view.RemovePromotion(item.key);
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.SetPriceOfIAPBanner(isAvailable);
        }

        [ListensTo(typeof(RewardUnlockedSignal))]
        public void OnRewardUnlocked(string key, int quantity)
        {
            view.OnRewardUnlocked(key, quantity);
        }

        [ListensTo(typeof(ShowAdSkippedDlgSignal))]
        public void OnShowAdSkippedDlg()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_AD_SKIPPED_DLG);
        }

        void OnUpgradeToPremiumClicked()
        {
            promotionsService.LoadSubscriptionPromotion();
            hAnalyticsService.LogEvent("upgrade_subscription_clicked", "menu", "lobby");
        }

        //[ListensTo(typeof(UpdateOfferDrawSignal))]
        //public void OfferDrawStatusUpdate(OfferDrawVO offerDrawVO)
        //{
        //    if (offerDrawVO.challengeId != view.matchInfoModel.activeChallengeId)
        //    {
        //        view.UpdateFriendBarDrawOfferStatus(offerDrawVO.status, offerDrawVO.offeredBy, offerDrawVO.opponentId);
        //        return;
        //    }
        //}

        //[ListensTo(typeof(RatingBoostAnimSignal))]
        public void OnRatingBoostAnimation(int ratingBoost)
        {
            view.RatingBoostAnimation(ratingBoost);
        }

        #region Tournament

        [ListensTo(typeof(UpdateTournamentsViewSignal))]
        public void UpdateTournamentView()
        {
            view.UpdateTournamentView();
        }

        public void OnJoinedTournamentClicked(JoinedTournamentData data)
        {
            updateTournamentLeaderboardPartialSignal.Dispatch(data.id);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS);
            getJoinedTournamentLeaderboardSignal.Dispatch(data.id, true);
        }

        public void OnOpenTournamentClicked(LiveTournamentData data)
        {
            if (data.concluded)
            {
                loadArenaSignal.Dispatch();
                updateBottomNavSignal.Dispatch(BottomNavView.ButtonId.Arena);
            }
            else
            {
                updateTournamentLeaderboardPartialSignal.Dispatch(data.shortCode);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS);
                fetchLiveTournamentRewardsSignal.Dispatch(data.shortCode);
            }
        }

        [ListensTo(typeof(DownloadableContentEventSignal))]
        public void OnDLCDownloadBegin(ContentType? contentType, ContentDownloadStatus status)
        {
            if (contentType != null && contentType.Equals(ContentType.Skins)
                && status.Equals(ContentDownloadStatus.Started) && view.IsVisible())
            {
                view.ShowProcessing(true,true);
            }
        }

        [ListensTo(typeof(DownloadableContentEventSignal))]
        public void OnDLCDownloadCompleted(ContentType? contentType, ContentDownloadStatus status)
        {
            if (contentType != null && contentType.Equals(ContentType.Skins)
                && !status.Equals(ContentDownloadStatus.Started) && view.IsVisible())
            {
                view.ShowProcessing(false,false);
            }
        }

        #endregion
    }
}

