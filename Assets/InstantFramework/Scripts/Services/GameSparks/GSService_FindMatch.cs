/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using strange.extensions.promise.impl;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using TurboLabz.TLUtils;
using GameSparks.Core;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> FindMatch(string action)
        {
            return new GSFindMatchRequest().Send(action);
        }
    }

    #region REQUEST

    public class GSFindMatchRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "FindMatch";
        const string ATT_ACTION = "action";

        public IPromise<BackendResult> Send(string action)
        {
            this.errorCode = BackendResult.MATCHMAKING_REQUEST_FAILED;

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_ACTION, action)
                .Send(null, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
