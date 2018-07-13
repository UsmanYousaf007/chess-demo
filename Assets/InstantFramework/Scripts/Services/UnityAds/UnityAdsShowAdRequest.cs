/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-08 23:17:03 UTC+05:00
/// 
/// @description
/// In the request classes for services the Send() method always returns a
/// promise with SomeResultType as a type parameter:
/// 
/// IPromise<BackendResult> Send()
/// IPromise<SocialServiceResult> Send()
/// 
/// We can return more data using more type parameters but if the returned type
/// is specific to the service itself then we need to shield the world outside
/// the service to not receive service specific type parameters. For that
/// purpose we use a callback as a parameter to the Send() method e.g.:
/// 
/// IPromise<BackendResult> Send(Action<SomeServiceSpecificType> callback)
/// 
/// instead of doing this
/// 
/// IPromise<BackendResult, SomeServiceSpecificType> Send()
/// 
/// However these would be valid:
/// 
/// IPromise<BackendResult, string> Send()
/// IPromise<BackendResult, SomeGenericType> Send()

using UnityEngine.Advertisements;

using strange.extensions.promise.api;
using strange.extensions.promise.impl;

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class UnityAdsShowAdRequest
    {
        private IPromise<AdsResult> promise = new Promise<AdsResult>();

        public IPromise<AdsResult> Send()
        {
            ShowAd();

            return promise;
        }

        public void ShowAd()
        {
            if (!Advertisement.IsReady())
            {
                DispatchResponse(AdsResult.FAILED);
                return;
            }

            ShowOptions showOptions = new ShowOptions();
            showOptions.resultCallback = OnShowAd;

            Advertisement.Show(showOptions);
        }

        public void OnShowAd(ShowResult result)
        {
            if (result == ShowResult.Finished)
            {
                DispatchResponse(AdsResult.FINISHED);
            }
            else if (result == ShowResult.Skipped)
            {
                DispatchResponse(AdsResult.SKIPPED);
            }
            else if (result == ShowResult.Failed)
            {
                DispatchResponse(AdsResult.FAILED);
            }
        }

        private void DispatchResponse(AdsResult result)
        {
            promise.Dispatch(result);
        }
    }
}
