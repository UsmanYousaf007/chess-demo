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

        public string downloadUrl { get; set; }

        public IPromise<BackendResult> GetDownloadUrl(string fileId)
        {
            return new GSFileDownloadRequest(GetRequestContext()).Send(fileId, OnDownloadUrlSuccess);
        }

        private void OnDownloadUrlSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;



            //findMatchRequestCompleteSignal.Dispatch(opponentStatus);
        }

        #region request
        public class GSFileDownloadRequest : GSFrameworkRequest
        {

            public GSFileDownloadRequest(GSFrameworkRequestContext context) : base(context) { }

            public IPromise<BackendResult> Send(string fileId, Action<object> onSuccess)
            {
                this.errorCode = BackendResult.DOWNLOAD_URL_GET_FAILED;
                this.onSuccess = onSuccess;

                new GetUploadedRequest()
                    .SetUploadId(fileId)
                    .Send(OnRequestSuccess, OnRequestFailure);

                return promise;
            }

        }
        #endregion
    }
}
