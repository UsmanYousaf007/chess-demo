/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using strange.extensions.promise.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
	public partial class GSService
	{
		public IPromise<BackendResult> ChangeUserDetails(string name)
		{
			return new GSChangeUserDetailsRequest().Send(name, OnChangeUserDetailsSuccess);
		}

		private void OnChangeUserDetailsSuccess(object r)
		{
			ChangeUserDetailsResponse response = (ChangeUserDetailsResponse)r;
			playerModel.name = response.ScriptData.GetString(GSBackendKeys.DISPLAY_NAME);
			playerModel.name = FormatUtil.SplitFirstLastNameInitial(playerModel.name);
            playerModel.editedName = playerModel.name;
        }
	}

	#region REQUEST

	public class GSChangeUserDetailsRequest : GSFrameworkRequest
	{
		public IPromise<BackendResult> Send(string name, Action<object> onSuccess)
		{
			this.onSuccess = onSuccess;
			this.errorCode = BackendResult.SET_PLAYER_SOCIAL_NAME_FAILED;

            GSRequestData scriptData = new GSRequestData();
            scriptData.AddBoolean("editedName", true);

            new ChangeUserDetailsRequest()
                .SetScriptData(scriptData)
                .SetDisplayName(name)
				.Send(OnRequestSuccess, OnRequestFailure);

			return promise;
		}
	}

	#endregion
}
