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
        //Dispatch Signals
        [Inject] public FindMatchRequestCompleteSignal findMatchRequestCompleteSignal { get; set; }

        public IPromise<BackendResult> FindMatch(string action)
        {
            var context = new GSFrameworkRequestContext
            {
                currentViewId = navigatorModel.currentViewId
            };

            return new GSFindMatchRequest(context).Send(action, OnFindMatchSuccess);
        }

        private void OnFindMatchSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;

            if (response.ScriptData == null)
            {
                return;
            }

            var opponentStatus = "available";

            if (response.ScriptData.ContainsKey("opponentStatus"))
            {
                opponentStatus = response.ScriptData.GetString("opponentStatus");
            }

            findMatchRequestCompleteSignal.Dispatch(opponentStatus);
        }
    }

    #region REQUEST

    public class GSFindMatchRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "FindMatch";
        const string ATT_ACTION = "action";

        public GSFindMatchRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string action, Action<object> onSuccess)
        {
            this.errorCode = BackendResult.MATCHMAKING_REQUEST_FAILED;
            this.onSuccess = onSuccess;

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_ACTION, action)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
