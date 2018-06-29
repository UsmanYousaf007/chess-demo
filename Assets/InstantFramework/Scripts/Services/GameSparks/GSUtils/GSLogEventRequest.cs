using GameSparks.Api.Requests;
using System;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TurboLabz.InstantFramework 
{
    public class GSLogEventRequest : GSFrameworkRequest
    {
        protected LogEventRequest request = new LogEventRequest();
        protected string key;
        protected BackendResult errorCode;
        protected Action<LogEventResponse> onSuccess;

        public IPromise<BackendResult> Send()
        {
            request.SetEventKey(key).Send(OnSuccess, OnFailure);
            return promise;
        }

        void OnSuccess(LogEventResponse response)
        {
            if (IsActive())
            {
                onSuccess(response); 
            }

            Dispatch(BackendResult.SUCCESS);
        }

        void OnFailure(LogEventResponse response)
        {
            Dispatch(errorCode);
        }
    }
}
