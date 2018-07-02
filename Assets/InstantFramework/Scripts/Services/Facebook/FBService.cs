/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using Facebook.Unity;
using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public class FBService : IFacebookService
    {
        public const string PLAYER_USER_ID_ALIAS = "me";

        public IPromise<FacebookResult> Init()
        {
            return new FBInitRequest().Send();
        }

        public IPromise<FacebookResult, string> Auth()
        {
            return new FBAuthRequest().Send();
        }

        public IPromise<FacebookResult, Sprite> GetSocialPic(string userId)
        {
            return new FBGetSocialPicRequest().Send(userId);
        }

        public IPromise<FacebookResult, string> GetSocialName()
        {
            return new FBSocialNameRequest().Send();
        }

        public bool isLoggedIn()
        {
            return FB.IsLoggedIn;
        }

        public string GetPlayerUserIdAlias()
        {
            return PLAYER_USER_ID_ALIAS;
        }
    }
}
