using GameSparks.Api.Requests;
using System;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using System.Collections.Generic;
using TurboLabz.TLUtils;
using GameSparks.Core;
using UnityEngine.Analytics;
using GameAnalyticsSDK;

namespace TurboLabz.InstantFramework 
{
    public class GSFrameworkRequest
    {
        static List<IPromise<BackendResult>> activePromises = new List<IPromise<BackendResult>>();

        protected IPromise<BackendResult> promise;
        protected Action<object, Action<object>> onSuccess;
        protected Action<object> onFailure;
        protected BackendResult errorCode;
        private GSFrameworkRequestContext context;
        protected Action<object> onSuccessExternal;

        public static void CancelRequestSession()
        {
            foreach (IPromise<BackendResult> activePromise in activePromises)
            {
                activePromise.Dispatch(BackendResult.CANCELED);
            }
            activePromises.Clear();
        }

        public GSFrameworkRequest(GSFrameworkRequestContext context)
        {
            promise = new Promise<BackendResult>();
            activePromises.Add(promise);
            this.context = context;
            onSuccessExternal = null;
        }

        protected void OnRequestSuccess(object response)
        {
            if (IsActive() && onSuccess != null)
            {
                onSuccess(response, onSuccessExternal);
            }

            Dispatch(BackendResult.SUCCESS);
        }

        protected void OnRequestFailure(object response)
        {
            LogUtil.Log("<--OnRequestFailure-->", "red");

            if (IsActive() && onFailure != null)
            {
                onFailure(response);
            }

            LogEventResponse logEventResponse = response as LogEventResponse;
            if (logEventResponse != null)
            {
                GSData error = logEventResponse.Errors;
                LogUtil.Log("OnRequestFailure error: " + error.JSON + " RequestId:" + logEventResponse.RequestId, "red");
                GameAnalytics.NewErrorEvent(GAErrorSeverity.Info, $"{errorCode}:{error.JSON}");
                string errorString = error.GetString("error");

                if (errorString == "timeout")
                {
                    LogUtil.Log("OnRequestFailure timeout error", "red");
                    Dispatch(BackendResult.REQUEST_TIMEOUT);
                    LogAnalytic(AnalyticsEventId.gs_call_fail, errorCode.ToString(), context.currentViewId.ToString(), "timeout_true");
                    return;
                }
                else
                {
                    errorString = error.GetString("authentication");
                    if (errorString != null && errorString.Equals("NOTAUTHORIZED"))
                    {
                        Dispatch(BackendResult.NOT_AUTHORIZED);
                        LogAnalytic(AnalyticsEventId.gs_call_fail, errorCode.ToString(), context.currentViewId.ToString(), "not_authorized");
                        return;
                    }
                    else
                    {
                        LogAnalytic(AnalyticsEventId.gs_call_fail, errorCode.ToString(), context.currentViewId.ToString(), "timeout_false");
                    }
                }
            }

            LogChallengeEventResponse r = response as LogChallengeEventResponse;
            if (r != null)
            {
                GSData error = r.Errors;
                LogUtil.Log("OnRequestFailure Challenge: " + error.JSON, "red");
                GameAnalytics.NewErrorEvent(GAErrorSeverity.Info, $"{errorCode}:{error.JSON}");
                string errorString = error.GetString("error");

                if (errorString == "timeout")
                {
                    LogUtil.Log("OnRequestFailure timeout error", "red");
                    Dispatch(BackendResult.REQUEST_TIMEOUT);
                    LogAnalytic(AnalyticsEventId.gs_call_fail, errorCode.ToString(), context.currentViewId.ToString(), "timeout_true");
                    return;
                }
                else
                {
                    errorString = error.GetString("authentication");
                    if (errorString != null && errorString.Equals("NOTAUTHORIZED"))
                    {
                        Dispatch(BackendResult.NOT_AUTHORIZED);
                        LogAnalytic(AnalyticsEventId.gs_call_fail, errorCode.ToString(), context.currentViewId.ToString(), "not_authorized");
                        return;
                    }
                    else
                    {
                        LogAnalytic(AnalyticsEventId.gs_call_fail, errorCode.ToString(), context.currentViewId.ToString(), "timeout_false");
                    }
                }

                string challengeInstanceId = error.GetString("challengeInstanceId");
                LogUtil.Log("OnRequestFailure challengeInstanceId: " + challengeInstanceId, "red");
                if (challengeInstanceId == "COMPLETE" || challengeInstanceId == "NOT_YOUR_TURN")
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

        public void LogAnalytic(AnalyticsEventId evt, params string[] param)
        {
            var evtStr = evt.ToString();
            if (param != null && param.Length > 0)
            {
                var paramDict = new Dictionary<string, object>();
                for (int i = 0; i < param.Length; i++)
                {
                    paramDict.Add($"P{i + 1}", param[i]);
                    evtStr += $":{param[i]}";
                }

                Analytics.CustomEvent(evt.ToString(), paramDict);
            }
            else
            {
                Analytics.CustomEvent(evt.ToString());
            }
            GameAnalytics.NewDesignEvent(evtStr);
        }
    }
}
