using System;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> GetDownloadableContentUrl(string shortCode, Action<object> onSuccessExternal)
        {
            return new GSDownloadableContentUrlRequest(GetRequestContext()).Send(shortCode, OnDownloadableUrlSuccess, onSuccessExternal);
        }

        private void OnDownloadableUrlSuccess(object data, Action<object> onSuccessExternal)
        {
            if (data != null)
            {
                GetDownloadableResponse response = (GetDownloadableResponse)data;
                //DownloadableItem item = new DownloadableItem
                //{
                //    url = response.Url,
                //    lastModified = response.LastModified,
                //    shortCode = response.ShortCode,
                //    size = response.Size
                //};
                onSuccessExternal?.Invoke(response.Url);
            }

        }

        #region request
        public class GSDownloadableContentUrlRequest : GSFrameworkRequest
        {

            public GSDownloadableContentUrlRequest(GSFrameworkRequestContext context) : base(context) { }

            public IPromise<BackendResult> Send(string shortCode, Action<object, Action<object>> onSuccess, Action<object> onSuccessExternal)
            {
                this.errorCode = BackendResult.DOWNLOAD_URL_GET_FAILED;
                this.onSuccess = onSuccess;
                this.onSuccessExternal = onSuccessExternal;

                new GetDownloadableRequest()
                    .SetShortCode(shortCode)
                    .Send(OnRequestSuccess, OnRequestFailure);

                return promise;
            }

        }
        #endregion
    }
}
