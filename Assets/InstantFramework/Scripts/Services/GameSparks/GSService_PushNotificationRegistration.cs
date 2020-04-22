/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> PushNotificationRegistration(string token)
        {
            return new GSPushNotificationRegistrationRequest(GetRequestContext()).Send(token);
        }
    }

    #region REQUEST

    public class GSPushNotificationRegistrationRequest : GSFrameworkRequest
    {
        const string DEVICE_OS_FCM = "fcm";

        public GSPushNotificationRegistrationRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string token)
        {
            this.errorCode = BackendResult.PUSH_NOTIFICATION_REGISTRATION_FAILED;

            new PushRegistrationRequest()
                .SetDeviceOS(DEVICE_OS_FCM)
                .SetPushId(token)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}