/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-11 18:04:06 UTC+05:00
/// 
/// @description
/// [add_description_here]

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        public bool isAuthenticated
        {
            get { return GS.Authenticated; }
        }

        public IPromise<BackendResult> AuthGuest()
        {
            return new GSAuthGuestRequest().Send().Then(OnAuth);
        }

        public IPromise<BackendResult> AuthFacebook()
        {
            return new GSAuthFacebookRequest().Send(facebookService).Then(OnAuth);
        }

        public IPromise<BackendResult> SetPlayerSocialName(string name)
        {
            return new GSSetPlayerSocialNameRequest().Send(name, OnSetPlayerSocialNameSuccess);
        }

        private void OnSetPlayerSocialNameSuccess(LogEventResponse response)
        {
            playerModel.name = response.ScriptData.GetString(GSBackendKeys.DISPLAY_NAME);
        }

        private void OnAuth(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                // You need to be authenticated in order to send ping requests
                // to the backend.
                // TODO: The AuthGuest/AuthFacebook method has a counterpart
                // which will be logout. When that happens, don't forget to stop
                // the pinger in that method.
                StartPostAuthProcesses();
            }
        }
    }
}
