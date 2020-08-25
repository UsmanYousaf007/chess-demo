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
    public class GetDownloadableContentRequest
    {
        private IPromise<BackendResult, AssetBundle> promise;
        private DownloadableContentEventSignal contentSignal;
        private IRoutineRunner routineRunner;
        private ContentType? contentType;
        private string downloadUrl;
        private string shortCode;
        private long lastModifiedTime;
        public delegate IPromise<BackendResult> DownloadableContentUrlFn(string x, Action<object> a);

        public GetDownloadableContentRequest()
        {
            routineRunner = new NormalRoutineRunner();
            promise = new Promise<BackendResult, AssetBundle>();
        }

        public GetDownloadableContentRequest(DownloadableContentEventSignal contentSignal, ContentType? contentType)
        {
            routineRunner = new NormalRoutineRunner();
            promise = new Promise<BackendResult, AssetBundle>();
            this.contentSignal = contentSignal;
            this.contentType = contentType;
        }

        public IPromise<BackendResult, AssetBundle> Send(DownloadableContentUrlFn getDownloadableContentUrlFn,
                                                        string downloadShortCode, string shortCode, long lastModifiedTime)
        {

            this.shortCode = shortCode;
            this.lastModifiedTime = lastModifiedTime;
            this.downloadUrl = null;
            contentSignal.Dispatch(contentType, ContentDownloadStatus.Started);

            if (getDownloadableContentUrlFn != null)
            {
                getDownloadableContentUrlFn(downloadShortCode, OnExternalSuccess).Then(OnUrlDownloadComplete);
            }
            else
            {
                this.downloadUrl = "FromCache";
                OnUrlDownloadComplete(BackendResult.SUCCESS);
            }

            return promise;
        }

        private void OnExternalSuccess(object o)
        {
            downloadUrl = (string)o;
        }

        private void OnUrlDownloadComplete(BackendResult result)
        {
            bool isValidUrl = !String.IsNullOrEmpty(downloadUrl);
            if ((result == BackendResult.SUCCESS && isValidUrl) || downloadUrl == "FromCache")
            {
                routineRunner.StartCoroutine(GetDownloadableContentCR(shortCode, downloadUrl, lastModifiedTime));
            }
            else if (result == BackendResult.DOWNLOAD_URL_GET_FAILED)
            {
                promise.Dispatch(BackendResult.DOWNLOADABLE_CONTENT_GET_FAILED, null);
                contentSignal.Dispatch(contentType, ContentDownloadStatus.Failed);

            }
        }

        private IEnumerator GetDownloadableContentCR(string shortCode, string url, long lastModifiedTime)
        {
            Hash128 hash = new Hash128();
            hash = Hash128.Compute(lastModifiedTime.ToString());

            var www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            string name = Application.companyName + "//" + Application.productName + "//" + "downlooadables//" + shortCode;
            www.downloadHandler = new DownloadHandlerAssetBundle(url, name, hash, 0);

            yield return www.SendWebRequest();

            if (string.IsNullOrEmpty(www.error))
            {
                while (!Caching.ready)
                {
                    yield return null;
                }

                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                promise.Dispatch(BackendResult.SUCCESS, bundle);
                contentSignal.Dispatch(contentType, ContentDownloadStatus.Completed);
            }
            else
            {
                promise.Dispatch(BackendResult.DOWNLOADABLE_CONTENT_GET_FAILED, null);
                contentSignal.Dispatch(contentType, ContentDownloadStatus.Failed);
            }
        }
    }
}
