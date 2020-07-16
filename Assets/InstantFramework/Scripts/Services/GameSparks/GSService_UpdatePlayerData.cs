/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> UpdatePlayerData()
        {
            return new GSUpdatePlayerDataRequest(GetRequestContext()).Send(playerModel);
        }
    }

    #region REQUEST

    public class GSUpdatePlayerDataRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "UpdatePlayerData";
        const string ATT_NOTIFICATION_COUNT = "notificationCount";
        const string ATT_JSON_DATA = "jsonData";

        public GSUpdatePlayerDataRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(IPlayerModel playerModel)
        {
            this.errorCode = BackendResult.UPDATE_PLAYER_DATA_FAILED;

            string uploadedPicId = playerModel.uploadedPicId ?? "";

            var jsonData = new GSRequestData()
                .AddNumber(ATT_NOTIFICATION_COUNT, playerModel.notificationCount)
                .AddNumber(GSBackendKeys.PlayerDetails.SUBSCRIPTION_EXPIRY_TIMESTAMP, playerModel.subscriptionExipryTimeStamp)
                .AddString(GSBackendKeys.PlayerDetails.SUBSCRIPTION_TYPE, playerModel.subscriptionType)
                .AddString(GSBackendKeys.PlayerDetails.UPLOADED_PIC_ID, uploadedPicId);

            new LogEventRequest()
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_JSON_DATA, jsonData)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
