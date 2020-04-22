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

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> GetGameStartTime(string challengeId)
        {
            var context = new GSFrameworkRequestContext
            {
                currentViewId = navigatorModel.currentViewId
            };

            return new GSGetGameStartTimeRequest(context).Send(challengeId, OnGetGameStartTimeSuccess);
        }

        private void OnGetGameStartTimeSuccess(object m)
        {
            ScriptMessage message = (ScriptMessage)m;
            matchInfoModel.activeMatch.gameStartTimeMilliseconds = message.Data.GetLong(GSBackendKeys.GAME_START_TIME).Value;
        }
    }

    #region REQUEST

    public class GSGetGameStartTimeRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "GetGameStartTime";
        const string ATT_CHALLENGE_ID = "challengeId";

        public GSGetGameStartTimeRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string challengeId, Action<object> onSuccess)
        {
            this.onSuccess = onSuccess;
            this.errorCode = BackendResult.GET_GAME_START_TIME_REQUEST_FAILED;
            AddListeners();

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_CHALLENGE_ID, challengeId)
                .Send(null, OnRequestFailure);

            return promise;
        }

        private void AddListeners()
        {
            ScriptMessage.Listener += OnScriptMessage;
        }

        private void RemoveListeners()
        {
            ScriptMessage.Listener -= OnScriptMessage;
        }

        private void OnScriptMessage(ScriptMessage message) 
        {
            if (message.ExtCode == GSBackendKeys.START_GAME_MESSAGE)
            {
                this.OnRequestSuccess(message);
            }
        }
    }

    #endregion
}
