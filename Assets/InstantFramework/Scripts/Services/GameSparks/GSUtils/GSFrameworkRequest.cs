using GameSparks.Api.Requests;
using System;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using System.Collections.Generic;
using TurboLabz.TLUtils;
using GameSparks.Core;

namespace TurboLabz.InstantFramework 
{
    public class GSFrameworkRequest
    {
        static List<IPromise<BackendResult>> activePromises = new List<IPromise<BackendResult>>();

        protected IPromise<BackendResult> promise;
        protected Action<object> onSuccess;
        protected BackendResult errorCode;

        public static void CancelRequestSession()
        {
            foreach (IPromise<BackendResult> activePromise in activePromises)
            {
                activePromise.Dispatch(BackendResult.CANCELED);
            }
            activePromises.Clear();
        }

        public GSFrameworkRequest()
        {
            promise = new Promise<BackendResult>();
            activePromises.Add(promise);
        }

        protected void OnRequestSuccess(object response)
        {

            if (IsActive() && onSuccess != null)
            {
                onSuccess(response);
            }

            Dispatch(BackendResult.SUCCESS);
        }

        protected void OnRequestFailure(object response)
        {
            LogUtil.Log("<--OnRequestFailure-->", "red");

            LogEventResponse logEventResponse = response as LogEventResponse;
            if (logEventResponse != null)
            {
                GSData error = logEventResponse.Errors;
                LogUtil.Log("OnRequestFailure error: " + error.JSON + " RequestId:" + logEventResponse.RequestId, "red");
                string errorString = error.GetString("error");

                if (errorString == "timeout")
                {
                    LogUtil.Log("OnRequestFailure timeout error", "red");
                    Dispatch(BackendResult.REQUEST_TIMEOUT);
                    return;
                }
            }

            LogChallengeEventResponse r = response as LogChallengeEventResponse;
            if (r != null)
            {
                GSData error = r.Errors;
                LogUtil.Log("OnRequestFailure Challenge: " + error.JSON, "red");

                string errorString = error.GetString("error");
                if (errorString == "timeout")
                {
                    LogUtil.Log("OnRequestFailure timeout error", "red");
                    Dispatch(BackendResult.REQUEST_TIMEOUT);
                    return;
                }

                string challengeInstanceId = error.GetString("challengeInstanceId");
                LogUtil.Log("OnRequestFailure challengeInstanceId: " + challengeInstanceId, "red");
                if (challengeInstanceId == "COMPLETE")
                {
                    Dispatch(BackendResult.CANCELED);
                    return;
                }
            }

            // Dispatch the error code
            Dispatch(errorCode);
        }

        protected void Dispatch(BackendResult result)
        {
            if (IsActive())
            {   
                Assertions.Assert((result != BackendResult.NONE), "Backend result not set in request.");

                promise.Dispatch(result);
                activePromises.Remove(promise);
                promise = null;
            }
        }

        protected bool IsActive()
        {
            return activePromises.IndexOf(promise) >= 0;
        }

    }
}
