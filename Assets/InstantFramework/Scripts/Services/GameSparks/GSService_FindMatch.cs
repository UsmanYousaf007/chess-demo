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
            return new GSFindMatchRequest(GetRequestContext()).Send(action, OnFindMatchSuccess, OnFindMatchFailed);
        }

        private void OnFindMatchSuccess(object r, Action<object> a)
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

        private void OnFindMatchFailed(object r)
        {
            var response = (LogEventResponse)r;
            var errorData = response.Errors;
            var errorString = errorData.GetString("error");

            if (errorString.Equals("currency4Insufficient"))
            {
                playerModel.coins = GSParser.GetSafeInt(errorData, "coins");
                updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            }
        }
    }

    #region REQUEST

    public class GSFindMatchRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "FindMatch";
        const string ATT_ACTION = "action";

        public GSFindMatchRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string action, Action<object, Action<object>> onSuccess, Action<object> onFailure)
        {
            this.errorCode = BackendResult.MATCHMAKING_REQUEST_FAILED;
            this.onSuccess = onSuccess;
            this.onFailure = onFailure;

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_ACTION, action)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
