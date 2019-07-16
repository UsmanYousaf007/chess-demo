/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using TurboLabz.TLUtils;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> SyncReconnectData()
        {
            return new GSSyncReconnectDataRequest().Send(OnSyncDataSuccess);
        }

        private void OnSyncDataSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;

            // Parse active quick match
            string challengeId = response.ScriptData.GetString("challengeId");
            LogUtil.Log("SyncReconnectData: RESUME ChallengeID = " + challengeId, "cyan");
            if (challengeId != null)
            {
                GSData challengeData = response.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
                ParseChallengeData(challengeId, challengeData);
                // The matchInfoModel.activeChallengeId is retained for the session and maintained by the client so it 
                // need not be set from the server. Do not set activeChallengeId here.
            }
        }
    }

    #region REQUEST

    public class GSSyncReconnectDataRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "SyncReconnectData";

        public IPromise<BackendResult> Send(Action<object> onSuccess)
        {
            this.onSuccess = onSuccess;

            new LogEventRequest()
                .SetEventKey(SHORT_CODE)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
