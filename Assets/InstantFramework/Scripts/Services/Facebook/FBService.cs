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
        IPromise<FacebookResult, Sprite, string> getPicPromise;

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
            if (IsInitialized())
            {
                return _GetSocialPic(facebookUserId, playerId);
            }
            else
            {
                getPicPromise = new Promise<FacebookResult, Sprite, string>();

                Init().Then((result) =>
                {
                    if (result == FacebookResult.SUCCESS)
                    {
                        _GetSocialPic(facebookUserId, playerId).Then((res, picture, id) =>
                        {
                            getPicPromise.Dispatch(res, picture, id);
                        });
                    }
                });
            }

            return getPicPromise;
        }

        private IPromise<FacebookResult, Sprite, string> _GetSocialPic(string facebookUserId, string playerId)
        {
            return new FBGetSocialPicRequest().Send(facebookUserId, playerId);
        }

        public IPromise<FacebookResult, string> GetSocialName()
        {
            return new FBSocialNameRequest().Send();
        }

        public bool isLoggedIn()
        {
            return FB.IsLoggedIn;
        }

        public bool IsInitialized()
        {
            return FB.IsInitialized;
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
