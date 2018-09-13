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

        public override void OnRegister()
        {
            view.Init();

            view.facebookButtonClickedSignal.AddListener(OnFacebookButtonClicked);
            view.reloadFriendsSignal.AddOnce(OnReloadFriends);
            view.showProfileDialogSignal.AddListener(OnShowProfileDialog);
            view.refreshCommunityButton.onClick.AddListener(OnRefreshCommunity);
            view.defaultInviteFriendsButton.onClick.AddListener(OnShareApp);
            view.inviteFriendsButton.onClick.AddListener(OnShareApp);
            view.playButtonClickedSignal.AddListener(OnPlayButtonClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.FRIENDS) 
            {
                view.Show();
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

        [ListensTo(typeof(UpdateFriendEloSignal))]
        public void OnUpdateFriendEloSignal(string playerId, int elo)
        {
            view.UpdateFriendElo(playerId, elo);
        }

        [ListensTo(typeof(UpdateFriendBarStatusSignal))]
        public void OnUpdateFriendBarStatus(LongPlayStatusVO vo)
        {
            view.UpdateFriendBarStatus(vo);
        }

        [ListensTo(typeof(FriendBarBusySignal))]
        public void OnFriendBarBusy(string playerId, bool busy)
        {
            view.UpdateFriendBarBusy(playerId, busy);
        }

        [ListensTo(typeof(SortFriendsSignal))]
        public void OnSortFriends()
        {
            view.Sort();
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
    }
}
