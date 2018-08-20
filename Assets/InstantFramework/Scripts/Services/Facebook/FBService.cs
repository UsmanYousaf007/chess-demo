/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using Facebook.Unity;
using strange.extensions.promise.api;
using System;
using System.Collections.Generic;
using strange.extensions.promise.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class FBService : IFacebookService
    {
        [Inject] public IPicsModel picsModel { get; set; }

        public IPromise<FacebookResult> Init()
        {
            return new FBInitRequest().Send();
        }

        public IPromise<FacebookResult, string> Auth()
        {
            return new FBAuthRequest().Send();
        }

        public IPromise<FacebookResult, Sprite, string> GetSocialPic(string facebookUserId, string playerId)
        {
            return new FBGetSocialPicRequest().Send(facebookUserId, playerId, OnGetSocialPic);
        }

        void OnGetSocialPic(string playerId, Sprite pic)
        {
            picsModel.SetPic(playerId, pic);
        }

        public IPromise<FacebookResult, string> GetSocialName()
        {
            return new FBSocialNameRequest().Send();
        }

        public bool isLoggedIn()
        {
            return FB.IsLoggedIn;
        }

        public string GetFacebookId()
        {
            return AccessToken.CurrentAccessToken.UserId;
        }

        public void LogOut()
        {
            FB.LogOut();
        }

		public string GetAccessToken()
		{
			if (isLoggedIn() == false) 
			{
				return null;
			}
			return AccessToken.CurrentAccessToken != null ? AccessToken.CurrentAccessToken.TokenString : null;
		}
    }
}
