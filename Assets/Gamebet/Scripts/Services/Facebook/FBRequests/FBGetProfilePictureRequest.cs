/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-31 11:35:22 UTC+05:00
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

using System.Collections;

using UnityEngine;

using Facebook.Unity;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class FBGetProfilePictureRequest : GSRequest
    {
        private const string PROFILE_PICTURE_TEXTURE_NAME = "FacebookProfilePicture";

        private IPromise<FacebookResult, Texture2D> promise = new Promise<FacebookResult, Texture2D>();

        // TODO: Remove this after bug is resolved. Patch for Facebook SDK bug.
        // Utils
        private IRoutineRunner routineRunner = new NormalRoutineRunner();

        public IPromise<FacebookResult, Texture2D> Send(string userId)
        {
            Assertions.Assert(FB.IsInitialized, "Facebook must already be initialized!");

            GetProfilePicture(userId);

            return promise;
        }

        /*private void GetProfilePicture(string userId)
        {
            // TODO(mubeeniqbal): Consider moving the image size to a constant.
            FB.API(userId + "/picture?width=256&height=256", HttpMethod.GET, OnGetProfilePicture);
        }*/

        // TODO: Remove this after bug is resolved. Patch for Facebook SDK bug.
        private void GetProfilePicture(string userId)
        {
            // TODO(mubeeniqbal): Consider moving the image size to a constant.
            FB.API(userId + "/picture?width=256&height=256&redirect=false", HttpMethod.GET, OnGetProfilePicture);
        }

        /*private void OnGetProfilePicture(IGraphResult result)
        {
            if (string.IsNullOrEmpty(result.Error)) // Success
            {
                Texture2D texture = result.Texture;
                texture.name = PROFILE_PICTURE_TEXTURE_NAME;
                DispatchResponse(FacebookResult.SUCCESS, texture);
            }
            else // Failure
            {
                DispatchResponse(FacebookResult.FAILURE, null);
            }
        }*/

        // TODO: Remove this after bug is resolved. Patch for Facebook SDK bug.
        private void OnGetProfilePicture(IGraphResult result)
        {
            if (string.IsNullOrEmpty(result.Error)) // Success
            {
                IDictionary data = result.ResultDictionary["data"] as IDictionary;
                string url = data["url"] as string;
                routineRunner.StartCoroutine(GetProfilePictureCR(url));
            }
            else // Failure
            {
                DispatchResponse(FacebookResult.FAILURE, null);
            }
        }

        // TODO: Remove this after bug is resolved. Patch for Facebook SDK bug.
        private IEnumerator GetProfilePictureCR(string url)
        {
            WWW www = new WWW(url);
            yield return www;

            if (string.IsNullOrEmpty(www.error)) // Success
            {
                Texture2D texture = www.texture;
                texture.name = PROFILE_PICTURE_TEXTURE_NAME;
                DispatchResponse(FacebookResult.SUCCESS, texture);
            }
            else // Failure
            {
                DispatchResponse(FacebookResult.FAILURE, null);
            }
        }

        private void DispatchResponse(FacebookResult result, Texture2D texture)
        {
            promise.Dispatch(result, texture);
        }
    }
}
