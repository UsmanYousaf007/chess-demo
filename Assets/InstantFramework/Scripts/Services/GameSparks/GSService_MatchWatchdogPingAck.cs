/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        private IPromise<BackendResult> MatchWatchdogPingAck()
        {
            return new GSMatchWatchdogPingAckRequest().Send();
        }

        private void OnMatchWatchdogPingAckSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;
        }
    }

    #region REQUEST

    public class GSMatchWatchdogPingAckRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "MatchWatchdogPingAck";

        public IPromise<BackendResult> Send()
        {
            new LogEventRequest()
                .SetEventKey(SHORT_CODE)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
