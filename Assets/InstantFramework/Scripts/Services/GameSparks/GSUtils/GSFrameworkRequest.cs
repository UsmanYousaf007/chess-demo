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
            LogChallengeEventResponse r = response as LogChallengeEventResponse;
            if (r != null)
            {
                string error = r.Errors.GetString("challengeInstanceId");
                if (error == "COMPLETE")
                {
                    Dispatch(BackendResult.CANCELED);
                }
            }
            else
            {
                Dispatch(errorCode);
            }
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
            return (promise != null);
        }

    }
}
