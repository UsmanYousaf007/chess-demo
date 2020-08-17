/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.Networking;

namespace TurboLabz.InstantFramework
{
    public class GetDownloadableContentRequest
    {
        [Inject] public IDownloadablesModel downloadablesModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        IPromise<BackendResult, AssetBundle> promise;
        private IRoutineRunner routineRunner;
        string downloadUrl;
        string shortCode;
        long lastModifiedTime;
        public delegate IPromise<BackendResult> DownloadableContentUrlFn(string x, Action<object> a);

        public GetDownloadableContentRequest()
        {

            routineRunner = new NormalRoutineRunner();
            promise = new Promise<BackendResult, AssetBundle>();
        }

        public IPromise<BackendResult, AssetBundle> Send(DownloadableContentUrlFn getDownloadableContentUrlFn, string shortCode, long lastModifiedTime)
        {
            this.shortCode = shortCode;
            this.lastModifiedTime = lastModifiedTime;
            getDownloadableContentUrlFn(shortCode, OnExternalSuccess).Then(OnUrlDownloadComplete);
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
                routineRunner.StartCoroutine(GetDownloadableConterCR(shortCode, downloadUrl, lastModifiedTime));
            }

            else if (result == BackendResult.DOWNLOAD_URL_GET_FAILED)
            {
                promise.Dispatch(BackendResult.DOWNLOADABLE_CONTENT_GET_FAILED, null);
            }
        }

        private IEnumerator GetDownloadableConterCR(string shortCode, string url, long lastModifiedTime)
        {
            /*subtracting 30 years*/
            lastModifiedTime -= 946707780000;

            /*from miliseconds to seconds*/
            lastModifiedTime = lastModifiedTime / 1000;

            /*from seconds to 15 min interval*/
            lastModifiedTime = lastModifiedTime / (60 * 15);
            uint versionNumber = (uint)lastModifiedTime;

            var www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            //{
            www.downloadHandler = new DownloadHandlerAssetBundle(url, versionNumber, 0);

            //}

            yield return www.SendWebRequest();

            if (string.IsNullOrEmpty(www.error))
            {

                while (!Caching.ready)
                {
                    yield return null;
                }

                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

                promise.Dispatch(BackendResult.SUCCESS, bundle);
            }
            else
            {
                promise.Dispatch(BackendResult.DOWNLOADABLE_CONTENT_GET_FAILED, null);
            }
        }

    }
}
