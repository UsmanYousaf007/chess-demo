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
        public IPromise<BackendResult> UpdatePlayerData(int notificationCount)
        {
            return new GSUpdatePlayerDataRequest().Send(notificationCount);
        }
    }

    #region REQUEST

    public class GSUpdatePlayerDataRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "UpdatePlayerData";
        const string ATT_NOTIFICATION_COUNT = "notificationCount";

        public IPromise<BackendResult> Send(int notificationCount)
        {
            this.errorCode = BackendResult.ACCEPT_FAILED;

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_NOTIFICATION_COUNT, notificationCount)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
