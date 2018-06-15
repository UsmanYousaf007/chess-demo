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
        public IPromise<BackendResult> FindMatch(string groupId)
        {
            return new GSFindMatchRequest().Send(groupId, OnFindMatchSuccess);
        }

        private void OnFindMatchSuccess(ChallengeStartedMessage message)
        {
         
        }
    }

    #region REQUEST

    public class GSFindMatchRequest
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();
        private Action<ChallengeStartedMessage> onSuccess;

        public IPromise<BackendResult> Send(string groupId, Action<ChallengeStartedMessage> onSuccess)
        {
            this.onSuccess = onSuccess;
            AddListeners();

            new LogEventRequest().SetEventKey("FindMatch")
                .SetEventAttribute("matchGroup", groupId)
                .Send((response) => {}, OnFailure);

            return promise;
        }

        private void OnFailure(LogEventResponse response)
        {
            DispatchResponse(BackendResult.MATCHMAKING_REQUEST_FAILED);
        }

        private void AddListeners()
        {
            ChallengeStartedMessage.Listener += OnChallengeStarted;
        }

        private void RemoveListeners()
        {
            ChallengeStartedMessage.Listener -= OnChallengeStarted;
        }

        private void OnChallengeStarted(ChallengeStartedMessage message) 
        {
            onSuccess(message);
            DispatchResponse(BackendResult.SUCCESS);
        }

        private void DispatchResponse(BackendResult result)
        {  
            RemoveListeners();
            promise.Dispatch(result);
        }
    }

    #endregion
}
