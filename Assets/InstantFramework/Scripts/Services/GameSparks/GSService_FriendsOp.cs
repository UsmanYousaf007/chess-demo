/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using GameSparks.Api.Requests;
using System;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
	public partial class GSService
	{
		public IPromise<BackendResult> FriendsOp(string op, string friendId)
		{
			return new GSFriendsOpRequest().Send(op, friendId, OnFriendOpSuccess);
		}

		private void OnFriendOpSuccess(object r)
		{
			LogEventResponse response = (LogEventResponse)r;

			// Populate friends data
			GSData friendsList = response.ScriptData.GetGSData(GSBackendKeys.FriendsOp.FRIENDS);
			playerModel.friends = PopulateFriends(friendsList);
		}
	}

	#region REQUEST

	public class GSFriendsOpRequest : GSFrameworkRequest
	{
		const string SHORT_CODE = "FriendsOp";
		const string ATT_FRIEND_ID = "friendId";
		const string ATT_OP = "op";

		public IPromise<BackendResult> Send(string op, string friendId, Action<object> onSuccess)
		{
			this.errorCode = BackendResult.FRIENDS_OP_FAILED;
			this.onSuccess = onSuccess;

			new LogEventRequest().SetEventKey(SHORT_CODE)
				.SetEventAttribute(ATT_OP, op)
				.SetEventAttribute(ATT_FRIEND_ID, friendId)
				.Send(OnRequestSuccess, OnRequestFailure);

			return promise;
		}
	}

	#endregion
}

