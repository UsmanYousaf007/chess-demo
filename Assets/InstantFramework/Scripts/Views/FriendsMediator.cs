﻿/// @license Propriety <http://license.url>
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
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public ShareAppSignal shareAppSignal { get; set; }
        [Inject] public TapLongMatchSignal tapLongMatchSignal { get; set; }
        [Inject] public SetActionCountSignal setActionCountSignal { get; set; }
        [Inject] public AcceptSignal acceptSignal { get; set; }
        [Inject] public DeclineSignal declineSignal { get; set; }
        [Inject] public CloseStripSignal closeStripSignal { get; set; }
        [Inject] public ResignSignal resignSignal { get; set; }
        [Inject] public RemoveCommunityFriendSignal removeCommunityFriendSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.facebookButtonClickedSignal.AddListener(OnFacebookButtonClicked);
            view.reloadFriendsSignal.AddOnce(OnReloadFriends);
            view.showProfileDialogSignal.AddListener(OnShowProfileDialog);
            view.refreshCommunityButton.onClick.AddListener(OnRefreshCommunity);
//            view.defaultInviteFriendsButton.onClick.AddListener(OnShareApp);
            view.playButtonClickedSignal.AddListener(OnPlayButtonClicked);
            view.actionCountUpdatedSignal.AddListener(OnActionCountUpdated);
            view.acceptButtonClickedSignal.AddListener(OnAcceptButtonClicked);
            view.declineButtonClickedSignal.AddListener(OnDeclineButtonClicked);
            view.cancelButtonClickedSignal.AddListener(OnCancelButtonClicked);
            view.okButtonClickedSignal.AddListener(OnOkButtonClicked);
            view.removeCommunityFriendSignal.AddListener(OnRemoveCommunityFriend);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.FRIENDS) 
            {
                view.Show();
                analyticsService.VisitFriends();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.FRIENDS)
            {
                view.Hide();
            }
        }

		[ListensTo(typeof(AddFriendsSignal))]
        public void OnUpdateFriends(Dictionary<string, Friend> friends, bool isCommunity)
		{
            view.AddFriends(friends, isCommunity);
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
        public void OnUpdateFriendOnlineStatusSignal(string friendId, bool isOnline)
        {
            view.UpdateFriendOnlineStatusSignal(friendId, isOnline);
        }

        [ListensTo(typeof(FriendBarBusySignal))]
        public void OnFriendBarBusy(string playerId, bool busy)
        {
            view.UpdateFriendBarBusy(playerId, busy);
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

        [ListensTo(typeof(FriendsShowConnectFacebookSignal))]
        public void OnFriendsConnectFacebook(bool showConnectInfo)
        {
            view.ShowConnectFacebook(showConnectInfo);
        }

        [ListensTo(typeof(AuthFacebookResultSignal))]
        public void OnAuthFacebookResult(bool isSuccessful, Sprite pic, string name)
        {
            if (view.IsVisible())
            {
                view.FacebookAuthResult(isSuccessful, pic, name);
            }
        }

        [ListensTo(typeof(ToggleFacebookButton))]
        public void OnToggleFacebookButton(bool toggle)
        {
            view.ToggleFacebookButton(toggle);
        }

        [ListensTo(typeof(AddUnreadMessagesToBarSignal))]
        public void OnAddUnreadMessages(string friendId)
        {
            view.AddUnreadMessages(friendId);
        }

        [ListensTo(typeof(ClearUnreadMessagesFromBarSignal))]
        public void OnClearUnreadMessages(string friendId)
        {
            view.ClearUnreadMessages(friendId);
        }

        private void OnFacebookButtonClicked()
        {
            authFacebookSignal.Dispatch();
        }

        private void OnReloadFriends()
        {
            loadFriendsSignal.Dispatch();
        }

        private void OnShowProfileDialog(string playerId)
        {
            showProfileDialogSignal.Dispatch(playerId);
        }

        private void OnRefreshCommunity()
        {
            analyticsService.TapCommunityRefresh();
           
            refreshCommunitySignal.Dispatch();
        }

        private void OnShareApp()
        {
            shareAppSignal.Dispatch();
        }

        private void OnPlayButtonClicked(string playerId)
        {
            tapLongMatchSignal.Dispatch(playerId);
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
            analyticsService.LongMatchCanceled();
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
    }
}
