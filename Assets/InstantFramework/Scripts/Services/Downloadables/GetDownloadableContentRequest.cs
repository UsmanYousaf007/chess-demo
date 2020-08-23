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
                                                        string shortCode, long lastModifiedTime)
        {
            DispatchSignal(ContentDownloadStatus.Started);
            TLUtils.LogUtil.Log("Initiated downloaded bundle request for - " + shortCode, "cyan");

            this.shortCode = shortCode;
            this.lastModifiedTime = lastModifiedTime;
            this.downloadUrl = null;

            if (getDownloadableContentUrlFn != null)
            {
                getDownloadableContentUrlFn(shortCode, OnExternalSuccess).Then(OnUrlDownloadComplete);
            }
            else
            {
                TLUtils.LogUtil.Log("Skip downloaded bundle URL request", "cyan");

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
                DispatchSignal(ContentDownloadStatus.Failed);

            }
        }

        private IEnumerator GetDownloadableContentCR(string shortCode, string url, long lastModifiedTime)
        {
            Hash128 hash = new Hash128();
            hash = Hash128.Compute(lastModifiedTime.ToString());

            var www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            string name = Application.companyName + "//" + Application.productName + "//" + "downlooadables//" + shortCode;
            www.downloadHandler = new DownloadHandlerAssetBundle(url, name, hash, 0);

            TLUtils.LogUtil.Log("Initiated downloaded bundle request - " + name + " hash: " + hash, "cyan");

            yield return www.SendWebRequest();

            if (string.IsNullOrEmpty(www.error))
            {
                while (!Caching.ready)
                {
                    yield return null;
                }

                TLUtils.LogUtil.Log("Fetch bundle from download handler - ", "cyan");


                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                Debug.Log("Bundle fetched: bundle name is " + bundle.name);
                TLUtils.LogUtil.Log("Downloaded bundle - " + shortCode, "cyan");
                DispatchSignal(ContentDownloadStatus.Completed);
                promise.Dispatch(BackendResult.SUCCESS, bundle);
            }
            else
            {
                TLUtils.LogUtil.Log("FAILED to to fetch bundle!! - ", "cyan");
                DispatchSignal(ContentDownloadStatus.Failed);
                promise.Dispatch(BackendResult.DOWNLOADABLE_CONTENT_GET_FAILED, null);
            }


        }

        private void DispatchSignal(ContentDownloadStatus status)
        {
            if (contentSignal != null)
            {
                //string stat = status.ToString();
                contentSignal.Dispatch(contentType, status);
            }
        }

    }
}
