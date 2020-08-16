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
        // Note: Do not inject a Strange context scope element. 

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

                this.downloadUrl = "na";
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
            if ((result == BackendResult.SUCCESS && isValidUrl) || downloadUrl == "na")
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
                TLUtils.LogUtil.Log("Downloaded bundle - " + shortCode, "cyan");
                promise.Dispatch(BackendResult.SUCCESS, bundle);
            }
            else
            {
                TLUtils.LogUtil.Log("FAILED to to fetch bundle!! - ", "cyan");

                promise.Dispatch(BackendResult.DOWNLOADABLE_CONTENT_GET_FAILED, null);
            }
        }

    }
}
