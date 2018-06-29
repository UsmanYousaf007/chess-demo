using GameSparks.Api.Requests;
using System;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework 
{
    public class GSFrameworkRequest
    {
        static List<IPromise<BackendResult>> activePromises = new List<IPromise<BackendResult>>();
        protected readonly IPromise<BackendResult> promise;

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

        protected void Dispatch(BackendResult result)
        {
            promise.Dispatch(result);
            activePromises.Remove(promise);
        }
    }
}
