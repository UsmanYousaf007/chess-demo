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
using TurboLabz.Multiplayer;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System;

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

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.playMultiplayerButtonClickedSignal.AddListener(OnQuickMatchBtnClicked);
            view.playCPUButtonClickedSignal.AddListener(OnPlayComputerMatchBtnClicked);

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

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.lobby, facebookService.isLoggedIn());
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY)
            {
                view.Hide();
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
            refreshCommunitySignal.Dispatch();

            // Analytics
            analyticsService.Event(AnalyticsEventId.tap_community_refresh);
        }

        private void OnShareApp()
        {
            shareAppSignal.Dispatch();
        }

        private void OnPlayButtonClicked(string playerId, bool isRanked)
        {
            tapLongMatchSignal.Dispatch(playerId, isRanked);
        }

        private void OnQuickMatchFriendButtonClicked(string playerId, bool isRanked)
        {
            analyticsService.Event(AnalyticsEventId.quickmatch_direct_request);

            var friend = playerModel.GetFriend(playerId);

            if (friend != null && friend.friendType.Equals(GSBackendKeys.Friend.TYPE_FAVOURITE))
            {
                analyticsService.Event(AnalyticsEventId.start_match_with_favourite);
            }

            FindMatchAction.Challenge(findMatchSignal, isRanked, playerId);
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
            startCPUGameSignal.Dispatch();
        }

        private void OnQuickMatchBtnClicked()
        {
            FindMatchAction.Random(findMatchSignal);
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

        [ListensTo(typeof(RemoveLobbyPromotionSignal))]
        public void OnRemoveLobbyPromotion(string key)
        {
            view.RemovePromotion(key);
        }

        [ListensTo(typeof(ReportLobbyPromotionAnalyticSingal))]
        public void OnReportLobbyPromotionAnalytic(string key, AnalyticsEventId eventId)
        {
            if (!view.IsVisible())
            {
                return;
            }

            view.ReportAnalytic(key, eventId);
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable, StoreVO storeVO)
        {
            view.SetPriceOfIAPBanner(isAvailable);
        }

        [ListensTo(typeof(ShowProcessingSignal))]
        public void OnShowProcessingUI(bool show)
        {
            view.ShowProcessing(show);
        }
    }
}

