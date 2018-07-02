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
            FB.API("/me?fields=first_name", HttpMethod.GET, OnAPICallback);
            return promise;
        }

        private void OnAPICallback(IGraphResult result)
        {
            if (string.IsNullOrEmpty(result.Error))
            {
                string name = result.ResultDictionary["first_name"].ToString();
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
