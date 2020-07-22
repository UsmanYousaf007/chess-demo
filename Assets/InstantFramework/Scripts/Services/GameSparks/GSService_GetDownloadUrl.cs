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
        public IPromise<BackendResult> GetDownloadUrl(string fileId, Action<object> onSuccessExternal)
        {
            return new GSFileDownloadRequest(GetRequestContext()).Send(fileId, OnDownloadUrlSuccess, onSuccessExternal);
        }

        private void OnDownloadUrlSuccess(object r, Action<object> onSuccessExternal)
        {
            GetUploadedResponse response = (GetUploadedResponse)r;

            if (onSuccessExternal != null)
            {
                onSuccessExternal(response.Url);
            }
        }

        #region request
        public class GSFileDownloadRequest : GSFrameworkRequest
        {

            public GSFileDownloadRequest(GSFrameworkRequestContext context) : base(context) { }

            public IPromise<BackendResult> Send(string fileId, Action<object, Action<object>> onSuccess, Action<object> onSuccessExternal)
            {
                this.errorCode = BackendResult.DOWNLOAD_URL_GET_FAILED;
                this.onSuccess = onSuccess;
                this.onSuccessExternal = onSuccessExternal;

                new GetUploadedRequest()
                    .SetUploadId(fileId)
                    .Send(OnRequestSuccess, OnRequestFailure);

                return promise;
            }

        }
        #endregion
    }
}
