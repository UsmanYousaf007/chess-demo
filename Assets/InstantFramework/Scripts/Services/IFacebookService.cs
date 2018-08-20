/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public interface IFacebookService
    {
        IPromise<FacebookResult> Init();
        IPromise<FacebookResult, string> Auth();
        IPromise<FacebookResult, Sprite, string> GetSocialPic(string facebookUserId, string playerId);
        IPromise<FacebookResult, string> GetSocialName();
        bool isLoggedIn();
        string GetFacebookId();
        void LogOut();
		string GetAccessToken();
    }
}
