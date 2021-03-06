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

using TurboLabz.Chess;
using TurboLabz.TLUtils;
using System.Collections.Generic;
using TurboLabz.InstantGame;
using TurboLabz.Multiplayer;
using System;

namespace TurboLabz.InstantFramework
{
    public class FriendsMediator : Mediator
    {
        // View injection
        [Inject] public FriendsView view { get; set; }

        // Dispatch signals
        [Inject] public AuthFaceBookSignal authFacebookSignal { get; set; }
        [Inject] public LoadFriendsSignal loadFriendsSignal { get; set; }
        [Inject] public ShowProfileDialogSignal showProfileDialogSignal { get; set; }
        [Inject] public ShareAppSignal shareAppSignal { get; set; }
        [Inject] public TapLongMatchSignal tapLongMatchSignal { get; set; }
        [Inject] public SetActionCountSignal setActionCountSignal { get; set; }
        [Inject] public AcceptSignal acceptSignal { get; set; }
        [Inject] public DeclineSignal declineSignal { get; set; }
        [Inject] public CloseStripSignal closeStripSignal { get; set; }
        [Inject] public ResignSignal resignSignal { get; set; }
        [Inject] public RemoveCommunityFriendSignal removeCommunityFriendSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public LoadChatSignal loadChatSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ManageBlockedFriendsSignal manageBlockedFriendsSignal { get; set; }
        [Inject] public AuthSignInWithAppleSignal authSignInWithAppleSignal { get; set; }
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }
        [Inject] public AddUnreadMessagesToBarSignal addUnreadMessagesSignal { get; set; }
        [Inject] public HideNotificationViewSignal hideNotificationViewSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IPromotionsService promotionsService { get; set; }
        [Inject] public IPreGameAdsService preGameAdsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IChatModel chatModel { get; set; }

        private bool fectchBlockedList = true;

        public override void OnRegister()
        {
            view.Init();

            view.facebookButtonClickedSignal.AddListener(OnFacebookButtonClicked);
            view.reloadFriendsSignal.AddOnce(OnReloadFriends);
            view.showProfileDialogSignal.AddListener(OnShowProfileDialog);
            view.inviteFriendSignal.AddListener(OnInviteFriend);
            view.playButtonClickedSignal.AddListener(OnPlayButtonClicked);
            view.quickMatchFriendButtonClickedSignal.AddListener(OnQuickMatchFriendButtonClicked);
            view.actionCountUpdatedSignal.AddListener(OnActionCountUpdated);
            view.acceptButtonClickedSignal.AddListener(OnAcceptButtonClicked);
            view.declineButtonClickedSignal.AddListener(OnDeclineButtonClicked);
            view.cancelButtonClickedSignal.AddListener(OnCancelButtonClicked);
            view.okButtonClickedSignal.AddListener(OnOkButtonClicked);
            view.removeCommunityFriendSignal.AddListener(OnRemoveCommunityFriend);
            view.showChatSignal.AddListener(OnShowChat);
            view.upgradeToPremiumButtonClickedSignal.AddListener(OnUpgradeToPremiumClicked);
            view.manageBlockedFriendsButtonClickedSignal.AddListener(OnManageBlockedFriends);
            view.signInWithAppleClicked.AddListener(OnSignInWithAppleButtonClicked);
            view.localRefreshFriends.AddListener(OnLocalRefreshFriends);
            view.hideNotificationSignal.AddListener(OnHideNotification);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.FRIENDS)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.friends, facebookService.isLoggedIn());
            }
            else if (viewId == NavigatorViewId.CREATE_MATCH_LIMIT_REACHED_DIALOG)
            {
                view.ShowCreateMatchLimitReacDlg();
            }
            else if (viewId == NavigatorViewId.FIND_YOUR_FRIEND_DLG)
            {
                view.ShowFriendsHelpDialog();
            }
        }

        [ListensTo(typeof(ShowFriendsHelpSignal))]
        public void OnShowFriendsHelpSignal()
        {
            //view.ShowFriendsHelpDialog();
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_FIND_YOUR_FRIEND_DLG);
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.FRIENDS)
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
            else if (viewId == NavigatorViewId.INVITE_DLG)
            {
                view.HideInviteDlg();
            }
            else if (viewId == NavigatorViewId.REMOVE_FRIEND_DLG)
            {
                view.HideRemoveCommunityFriendDlg();
            }
            else if (viewId == NavigatorViewId.FIND_YOUR_FRIEND_DLG)
            {
                view.HideFriendsHelpDialog();
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
            view.UpdateFriendOnlineStatusSignal(vo.playerId, vo.isOnline);
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

        [ListensTo(typeof(SortSearchedSignal))]
        public void OnSortSearched(bool isSuccess)
        {
            if (view.IsVisible())
            {
                view.SortSearched(isSuccess);
            }
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

        [ListensTo(typeof(FriendsShowConnectFacebookSignal))]
        public void OnFriendsConnectFacebook(bool showConnectInfo)
        {
            view.ShowConnectFacebook(showConnectInfo);
        }

        [ListensTo(typeof(AuthFacebookResultSignal))]
        public void OnAuthFacebookResult(AuthFacebookResultVO vo)
        {
            if (view.IsVisible())
            {
                view.FacebookAuthResult(vo);
            }
        }

        [ListensTo(typeof(AuthSignInWithAppleResultSignal))]
        public void OnAuthSignInWithAppleSignal(AuthSignInWIthAppleResultVO vo)
        {
            if (view.IsVisible())
            {
                view.SignInWithAppleResult(vo);
            }
        }

        [ListensTo(typeof(SignOutSocialAccountSignal))]
        public void OnSignOutSocialAccount()
        {
            view.SignOutSocialAccount();
        }

        [ListensTo(typeof(ToggleFacebookButton))]
        public void OnToggleFacebookButton(bool toggle)
        {
            view.ToggleFacebookButton(toggle);
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

        [ListensTo(typeof(CancelSearchResultSignal))]
        public void cancelSearchResultSignal()
        {
            view.CancelSearchResult();
        }

        [ListensTo(typeof(SkinUpdatedSignal))]
        public void OnSkinUpdated()
        {
            view.UpdateBarsSkin();
        }

        [ListensTo(typeof(DownloadableContentEventSignal))]
        public void OnDLCDownloadBegin(ContentType? contentType, ContentDownloadStatus status)
        {
            if (contentType != null && contentType.Equals(ContentType.Skins)
                && status.Equals(ContentDownloadStatus.Started) && view.IsVisible())
            {
                view.ShowProcessing(true, true);
            }
        }

        [ListensTo(typeof(DownloadableContentEventSignal))]
        public void OnDLCDownloadCompleted(ContentType? contentType, ContentDownloadStatus status)
        {
            if (contentType != null && contentType.Equals(ContentType.Skins)
                && !status.Equals(ContentDownloadStatus.Started) && view.IsVisible())
            {
                view.ShowProcessing(false, false);
            }
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

        private void OnInviteFriend()
        {
            shareAppSignal.Dispatch();
        }

        private void OnPlayButtonClicked(string playerId, bool isRanked)
        {
            preGameAdsService.ShowPreGameAd().Then(() => tapLongMatchSignal.Dispatch(playerId, isRanked));
        }

        private void OnHideNotification()
        {
            hideNotificationViewSignal.Dispatch();
        }

        private void OnQuickMatchFriendButtonClicked(string playerId, bool isRanked, string actionCode)
        {
            preGameAdsService.ShowPreGameAd(actionCode).Then(() => FindMatchAction.Challenge(findMatchSignal, isRanked, playerId, actionCode)); 
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

        void OnUpgradeToPremiumClicked()
        {
            promotionsService.LoadSubscriptionPromotion();
            hAnalyticsService.LogEvent("upgrade_subscription_clicked", "menu", "friends");
        }

        private void OnManageBlockedFriends()
        {
            manageBlockedFriendsSignal.Dispatch(string.Empty, fectchBlockedList);
            fectchBlockedList = false;
        }

        [ListensTo(typeof(UpdateOfferDrawSignal))]
        public void OfferDrawStatusUpdate(OfferDrawVO offerDrawVO)
        {
            if (offerDrawVO.challengeId != view.matchInfoModel.activeChallengeId)
            {
                view.UpdateFriendBarDrawOfferStatus(offerDrawVO.status, offerDrawVO.offeredBy, offerDrawVO.opponentId);
                return;
            }

        }

        private void OnSignInWithAppleButtonClicked()
        {
            authSignInWithAppleSignal.Dispatch();
        }

        private void OnLocalRefreshFriends()
        {
            view.AddFriends(playerModel.friends, false, false);

            foreach (string key in playerModel.friends.Keys)
            {
                updateFriendBarSignal.Dispatch(playerModel.friends[key], key);

                if (key != null && chatModel.hasUnreadMessages.ContainsKey(key))
                {
                    addUnreadMessagesSignal.Dispatch(key, chatModel.hasUnreadMessages[key]);
                }
            }

            view.SortFriends();
        }
    }
}
