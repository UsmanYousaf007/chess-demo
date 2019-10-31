/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using UnityEngine;
using Facebook.Unity;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using TurboLabz.TLUtils;
using System;

namespace TurboLabz.InstantFramework
{
    public class FBGetSocialPicRequest
    {
        private const string PLAYER_SOCIAL_PIC = "PlayerSocialPic";
        private IPromise<FacebookResult, Sprite, string> promise = new Promise<FacebookResult, Sprite, string>();
        private IRoutineRunner routineRunner = new NormalRoutineRunner();
        private string playerId;

        public IPromise<FacebookResult, Sprite, string> Send(string facebookUserId, string playerId)
        {
            this.playerId = playerId;
            GetProfilePicture(facebookUserId);

            return promise;
        }

        private void GetProfilePicture(string facebookUserId)
        {
            if (!string.IsNullOrEmpty(facebookUserId))
            {
                FB.API(facebookUserId + "/picture?width=256&height=256&redirect=false", HttpMethod.GET, OnGetProfilePicture);
            }
        }

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
            
        private IEnumerator GetProfilePictureCR(string url)
        {
            WWW www = new WWW(url);
            yield return www;

            if (string.IsNullOrEmpty(www.error)) // Success
            {
                Texture2D texture = www.texture;
                texture.name = PLAYER_SOCIAL_PIC;

                Sprite sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                sprite.name = texture.name;

                DispatchResponse(FacebookResult.SUCCESS, sprite);
            }
            else // Failure
            {
                DispatchResponse(FacebookResult.FAILURE, null);
            }
        }

        private void DispatchResponse(FacebookResult result, Sprite sprite)
        {
            promise.Dispatch(result, sprite, playerId);
        }
    }
}
