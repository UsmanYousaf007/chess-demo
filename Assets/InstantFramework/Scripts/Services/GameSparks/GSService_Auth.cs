/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public bool isAuthenticated
        {
            get { return GS.Authenticated; }
        }

        public IPromise<BackendResult> AuthGuest()
        {
            TurboLabz.TLUtils.LogUtil.Log("GSService_AuthGuest", "yellow");

            return new GSAuthGuestRequest().Send();
        }

        public IPromise<BackendResult> AuthFacebook()
        {
            return new GSAuthFacebookRequest().Send(facebookService);
        }

        public IPromise<BackendResult> SetPlayerSocialName(string name)
        {
            return new GSSetPlayerSocialNameRequest().Send(name, OnSetPlayerSocialNameSuccess);
        }

        private void OnSetPlayerSocialNameSuccess(LogEventResponse response)
        {
            playerModel.name = response.ScriptData.GetString(GSBackendKeys.DISPLAY_NAME);
        }
    }
}
