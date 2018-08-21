/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using GameSparks.Api.Requests;
using System;
using GameSparks.Core;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
	public partial class GSService
	{
        public IPromise<BackendResult> FriendsOpBlock(string friendId) { return new GSFriendsOpRequest().Send("block", friendId, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpFriends() { return new GSFriendsOpRequest().Send("friends", null, OnFriendOpSuccess); } 
        public IPromise<BackendResult> FriendsOpRefresh() { return new GSFriendsOpRequest().Send("refresh", null, OnFriendOpSuccess); } 
        public IPromise<BackendResult> FriendsOpCommunity() { return new GSFriendsOpRequest().Send("community", null, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpRegCommunity() { return new GSFriendsOpRequest().Send("regcommunity", null, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpAdd(string friendId) { return new GSFriendsOpRequest().Send("add", friendId, OnFriendOpSuccess, facebookService.GetAccessToken()); } 

		private void OnFriendOpSuccess(object r)
		{
			LogEventResponse response = (LogEventResponse)r;

			// Populate friends data
			GSData friendsList = response.ScriptData.GetGSData(GSBackendKeys.FriendsOp.FRIENDS);
            if (friendsList != null)
            {
                PopulateFriends(playerModel.friends, friendsList);
                GSParser.LogFriends("friends", playerModel.friends);
            }
            // Populate blocked friends data
			GSData blockedList = response.ScriptData.GetGSData(GSBackendKeys.FriendsOp.BLOCKED);
            if (blockedList != null)
            {
                PopulateFriends(playerModel.blocked, blockedList);
                GSParser.LogFriends("blocked", playerModel.blocked);
            }
            // Populate community suggested friends data
            GSData communityList = response.ScriptData.GetGSData(GSBackendKeys.FriendsOp.COMMUNITY);
            if (communityList != null)
            {
                PopulateFriends(playerModel.community, communityList);
                GSParser.LogFriends("community", playerModel.community);
            }
            // Friend added
            GSData friendsData = response.ScriptData.GetGSData(GSBackendKeys.FriendsOp.ADD);
            if (friendsData != null)
            {
                foreach(KeyValuePair<string, object> obj in friendsData.BaseData)
                {
                    GSData friendData = (GSData)obj.Value;
                    string friendId = obj.Key;
                    Friend friend = LoadFriend(friendId, friendData);
                    playerModel.friends.Add(friendId, friend);
                }
            }
		}
	}

	#region REQUEST

	public class GSFriendsOpRequest : GSFrameworkRequest
	{
		const string SHORT_CODE = "FriendsOp";
		const string ATT_FRIEND_ID = "friendId";
		const string ATT_OP = "op";
        const string ATT_FBTOKEN = "fbToken";

        public IPromise<BackendResult> Send(string op, string friendId, Action<object> onSuccess, string fbToken = null)
		{
			this.errorCode = BackendResult.FRIENDS_OP_FAILED;
			this.onSuccess = onSuccess;

			new LogEventRequest().SetEventKey(SHORT_CODE)
				.SetEventAttribute(ATT_OP, op)
				.SetEventAttribute(ATT_FRIEND_ID, friendId)
                .SetEventAttribute(ATT_FBTOKEN, fbToken)
				.Send(OnRequestSuccess, OnRequestFailure);

			return promise;
		}
	}

	#endregion
}

