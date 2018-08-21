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
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantGame
{
    public class FriendsMediator : Mediator
    {
        // View injection
        [Inject] public FriendsView view { get; set; }

        // Dispatch signals
        [Inject] public AuthFaceBookSignal authFacebookSignal { get; set; }
        [Inject] public LoadFriendsSignal loadFriendsSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.facebookButtonClickedSignal.AddListener(OnFacebookButtonClicked);
            view.reloadFriendsSignal.AddOnce(OnReloadFriends);
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

		[ListensTo(typeof(AddFriendSignal))]
		public void OnUpdateFriends(Friend friend)
		{
			view.AddFriend(friend);
		}

        [ListensTo(typeof(UpdateFriendPicSignal))]
        public void OnUpdateFriendPic(string playerId, Sprite sprite)
        {
            view.UpdateFriendPic(playerId, sprite);
        }

        [ListensTo(typeof(ClearCommunitySignal))]
        public void OnClearCommunity()
        {
            view.ClearCommunity();
        }

        [ListensTo(typeof(FriendsConnectFacebookSignal))]
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

        private void OnFacebookButtonClicked()
        {
            authFacebookSignal.Dispatch();
        }

        private void OnReloadFriends()
        {
            loadFriendsSignal.Dispatch();
        }

    }
}
