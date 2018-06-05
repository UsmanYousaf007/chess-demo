/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using Facebook.Unity;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class FBInitRequest
    {
        private IPromise<FacebookResult> promise = new Promise<FacebookResult>();

        public IPromise<FacebookResult> Send()
        {
            Assertions.Assert(!FB.IsInitialized, "Facebook must not already be initialized!");

            FB.Init(OnInit);
            return promise;
        }

        private void OnInit()
        {
            if (FB.IsInitialized)
            {
                DispatchResponse(FacebookResult.SUCCESS);
            }
            else
            {
                OnInitFailure();
            }
        }

        private void OnInitFailure()
        {
            LogUtil.Log(this.GetType().Name + ": Facebook init failure", "red");
            DispatchResponse(FacebookResult.FAILURE);
        }

        private void DispatchResponse(FacebookResult result)
        {
            promise.Dispatch(result);
        }
    }
}
