/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using Facebook.Unity;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using TurboLabz.TLUtils;
using System.Collections;

namespace TurboLabz.InstantFramework
{
    public class FBSocialNameRequest
    {
        private IPromise<FacebookResult, string> promise = new Promise<FacebookResult, string>();

        public IPromise<FacebookResult, string> Send()
        {
            FB.API("/me?fields=first_name,last_name", HttpMethod.GET, OnAPICallback);
            return promise;
        }

        private void OnAPICallback(IGraphResult result)
        {
            // Player name format <first name><space><last initial>

            if (string.IsNullOrEmpty(result.Error))
            {
                string first = result.ResultDictionary["first_name"].ToString();
                string last = result.ResultDictionary["last_name"].ToString();
                string lastInitial = last.Length != 0 ? last[0].ToString() : last;
                string name = lastInitial.Length != 0  ? first + " " + last[0] + "." : first;

                promise.Dispatch(FacebookResult.SUCCESS, name);
            }
            else
            {
                LogUtil.Log(this.GetType().Name + ": Facebook name request failure:" + result.ToString(), "red");
                promise.Dispatch(FacebookResult.FAILURE, null);
            }
        }
    }
}
