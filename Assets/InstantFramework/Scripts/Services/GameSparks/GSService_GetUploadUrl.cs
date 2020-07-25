using System;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public string uploadUrl { get; set; }
        public IPromise<BackendResult> GetUploadUrl()
        {
            return new GSFileUploadRequest(GetRequestContext()).Send(OnUploadUrlSuccess);
        }

        private void OnUploadUrlSuccess(object r, Action<object> a)
        {
            GetUploadUrlResponse response = (GetUploadUrlResponse)r;
            uploadUrl = response.Url;
        }

        #region request
        public class GSFileUploadRequest : GSFrameworkRequest
        {
            public GSFileUploadRequest(GSFrameworkRequestContext context) : base(context) { }

            public IPromise<BackendResult> Send(Action<object, Action<object>> onSuccess)
            {
                this.errorCode = BackendResult.UPLOAD_URL_GET_FAILED;
                this.onSuccess = onSuccess;

                new GetUploadUrlRequest()
                    .Send(OnRequestSuccess, OnRequestFailure);

                return promise;
            }

        }
        #endregion
    }
}
