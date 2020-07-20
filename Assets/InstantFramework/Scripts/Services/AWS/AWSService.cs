/// @license Propriety <http://license.url>
/// @copyright Copyright (C) DefaultCompany 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TurboLabz.InstantFramework
{
    public partial class AWSService : IAWSService
    {
        [Inject] public IRoutineRunner routineRunner { get; set; }

        protected IPromise<BackendResult, string> promise = new Promise<BackendResult, string>();

        public IPromise<BackendResult, string> GetSignedUrl(string unsignedURL)
        {
            routineRunner.StartCoroutine(GetSignedUrlRequest(unsignedURL));

            return promise;
        }

        #region REQUEST

        IEnumerator GetSignedUrlRequest(string unsignedURL)
        {
            string urlWithQueryParams = SignedURLAPI.REQUEST_URL + "?url=" + unsignedURL;

            UnityWebRequest www = UnityWebRequest.Get(urlWithQueryParams);
            www.SetRequestHeader("x-api-key", SignedURLAPI.API_KEY);

            yield return www.SendWebRequest();

            if (www.isDone)
            {
                var bodyDict = MiniJSON.Json.Deserialize(www.downloadHandler.text) as Dictionary<string, object>;

                if (bodyDict.ContainsKey("body"))
                {
                    promise.Dispatch(BackendResult.SUCCESS, (string)bodyDict["body"]);
                }
                else
                {
                    promise.Dispatch(BackendResult.ACCEPT_FAILED, null);
                }
            }
            else
            {
                promise.Dispatch(BackendResult.ACCEPT_FAILED, null);
            }
        }

        #endregion
    }
}
