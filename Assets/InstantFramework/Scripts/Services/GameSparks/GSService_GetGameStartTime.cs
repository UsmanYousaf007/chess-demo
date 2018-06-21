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
            return new GSGetGameStartTimeRequest().Send(challengeId, OnGetGameStartTimeSuccess);
        }

        private void OnGetGameStartTimeSuccess(ScriptMessage message)
        {
            matchInfoModel.gameStartTimeMilliseconds = message.Data.GetLong(GSBackendKeys.GAME_START_TIME).Value;
        }
    }

    #region REQUEST

    public class GSGetGameStartTimeRequest
    {
        private readonly IPromise<BackendResult> promise = new Promise<BackendResult>();
        private Action<ScriptMessage> successCallback;

        public IPromise<BackendResult> Send(string challengeId, Action<ScriptMessage> successCallback)
        {
            this.successCallback = successCallback;
            AddListeners();

            const string eventKey = "GetGameStartTime";
            const string attributeKey = "challengeId";

            new LogEventRequest().SetEventKey(eventKey)
                .SetEventAttribute(attributeKey, challengeId)
                .Send(null, OnFailure);

            return promise;
        }

        private void OnFailure(LogEventResponse response)
        {
            RemoveListeners();
            DispatchResponse(BackendResult.GET_GAME_START_TIME_REQUEST_FAILED);
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
                successCallback(message);
                DispatchResponse(BackendResult.SUCCESS);
            }
        }

        private void DispatchResponse(BackendResult result)
        {  
            RemoveListeners();
            promise.Dispatch(result);
        }
    }

    #endregion
}
