/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.Networking;

namespace TurboLabz.InstantFramework
{
    public class GetProfilePicRequest
    {
        IPromise<BackendResult, Sprite, string> promise;
        private IRoutineRunner routineRunner;
        string playerId;
        string downloadUrl;

        public delegate IPromise<BackendResult> DownloadUrlFn(string x, Action<object> a);

        public GetProfilePicRequest()
        {
            routineRunner = new NormalRoutineRunner();
            promise = new Promise<BackendResult, Sprite, string>();
        }

        public IPromise<BackendResult, Sprite, string> Send(DownloadUrlFn getDownloadUrlFn, string playerId, string uploadedPicId)
        {
            this.playerId = playerId;
            getDownloadUrlFn(uploadedPicId, OnExternalSuccess).Then(OnUrlDownloadComplete);
            return promise;
        }

        private void OnExternalSuccess(object o)
        {
            downloadUrl = (string)o;
        }

        private void OnUrlDownloadComplete(BackendResult result)
        {
            bool isValidUrl = !String.IsNullOrEmpty(downloadUrl);
            if (result == BackendResult.SUCCESS && isValidUrl)
            {
                routineRunner.StartCoroutine(GetProfilePictureCR(downloadUrl));
            }

            else if (result == BackendResult.DOWNLOAD_URL_GET_FAILED)
            {
                promise.Dispatch(BackendResult.DOWNLOAD_PICTURE_FAILED,null,playerId);                
            }
            
        }

        private IEnumerator GetProfilePictureCR(string url)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (string.IsNullOrEmpty(www.error))
            {
                var texture = DownloadHandlerTexture.GetContent(www);
              
                    Sprite sprite = Sprite.Create(texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));
                    sprite.name = texture.name;

                    promise.Dispatch(BackendResult.SUCCESS, sprite,playerId);
            }
            else
            {
                promise.Dispatch(BackendResult.DOWNLOAD_PICTURE_FAILED, null,playerId);
            }
        }

    }
}
