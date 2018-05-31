/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;

using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TurboLabz.InstantFramework
{
    public class GSClaimRewardRequest : GSRequest
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();
        private Action<LogEventResponse> successCallback;

        public IPromise<BackendResult> Send(string rewardType, Action<LogEventResponse> successCallback)
        {
            GSRequestSession.Instance.AddRequest(this);
            this.successCallback = successCallback;
            new LogEventRequest().SetEventKey(GSEventData.ClaimReward.EVT_KEY)
                .SetEventAttribute(GSEventData.ClaimReward.EVT_REWARD_TYPE, rewardType)
                .Send(OnSuccess, OnFailure);

            return promise;
        }

        public override void Expire()
        {
            base.Expire();
            promise.Dispatch(BackendResult.EXPIRED_RESPONSE);
        }

        private void OnSuccess(LogEventResponse response)
        {
            if (!isExpired)
            {
                successCallback(response);
                DispatchResponse(BackendResult.SUCCESS); 
            }
        }

        private void OnFailure(LogEventResponse response)
        {
            if (!isExpired)
            {
                DispatchResponse(BackendResult.CLAIM_REWARD_REQUEST_FAILED);
            }
        }

        private void DispatchResponse(BackendResult result)
        {  
            promise.Dispatch(result);
            GSRequestSession.Instance.RemoveRequest(this);
        }
    }
}
