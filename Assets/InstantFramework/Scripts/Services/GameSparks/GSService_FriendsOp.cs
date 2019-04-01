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
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
	public partial class GSService
	{
        public IPromise<BackendResult> FriendsOpBlock(string friendId) { return new GSFriendsOpRequest().Send("block", friendId, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpFriends() { return new GSFriendsOpRequest().Send("friends", null, OnFriendOpSuccess); } 
        public IPromise<BackendResult> FriendsOpRefresh() { return new GSFriendsOpRequest().Send("refresh", null, OnFriendOpSuccess); } 
        public IPromise<BackendResult> FriendsOpCommunity() { return new GSFriendsOpRequest().Send("community", null, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpRegCommunity() { return new GSFriendsOpRequest().Send("regcommunity", null, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpAdd(string friendId) { return new GSFriendsOpRequest().Send("add", friendId, OnFriendOpSuccess, 0, facebookService.GetAccessToken()); } 
        public IPromise<BackendResult> FriendsOpInitialize() { return new GSFriendsOpRequest().Send("initialize", null, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpRemove(string friendId) { return new GSFriendsOpRequest().Send("remove", friendId, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpSearch(string matchString, int skip) { return new GSFriendsOpRequest().Send("search", matchString, OnFriendOpSuccess, skip); }

        private void OnFriendOpSuccess(object r)
		{
			LogEventResponse response = (LogEventResponse)r;

            if (response.ScriptData == null)
            {
                return;
            }

			// Populate friends data
            GSData friendsList = response.ScriptData.GetGSData(GSBackendKeys.FriendsOp.FRIENDS);
            if (friendsList != null)
            {
                PopulateFriends(playerModel.friends, friendsList);
                //GSParser.LogFriends("friends", playerModel.friends);
                ParseActiveChallenges(response.ScriptData);
            }

            // Populate blocked friends data
			GSData blockedList = response.ScriptData.GetGSData(GSBackendKeys.FriendsOp.BLOCKED);
            if (blockedList != null)
            {
                PopulateFriends(playerModel.blocked, blockedList, true);
                //GSParser.LogFriends("blocked", playerModel.blocked);
            }

            // Populate community suggested friends data
            GSData communityList = response.ScriptData.GetGSData(GSBackendKeys.FriendsOp.COMMUNITY);
            if (communityList != null)
            {
                PopulateFriends(playerModel.community, communityList);
                //GSParser.LogFriends("community", playerModel.community);
            }

            // Populate community suggested friends data
            GSData searchList = response.ScriptData.GetGSData(GSBackendKeys.FriendsOp.SEARCH);
            if (searchList != null)
            {
                PopulateFriends(playerModel.search, searchList);
                GSParser.LogFriends("search", playerModel.search);
            }

            // Friend added
            GSData friendDict = response.ScriptData.GetGSData(GSBackendKeys.FriendsOp.ADD);
            AddFriend(friendDict);

            // Friend blocked
            string blockedId = response.ScriptData.GetString(GSBackendKeys.FriendsOp.BLOCK);
            if (blockedId != null)
            {
                Friend friend = playerModel.friends[blockedId];
                playerModel.blocked.Add(blockedId, friend);
                playerModel.friends.Remove(blockedId);
            }

            // Populate chat if any
            GSData chatData = response.ScriptData.GetGSData(GSBackendKeys.CHAT);
            if (chatData != null)
            {
                FillChatModel(chatData);
            }
        }

        private void AddFriend(GSData friendDict)
        {
            if (friendDict != null)
            {
                foreach(KeyValuePair<string, object> obj in friendDict.BaseData)
                {
                    GSData friendData = (GSData)obj.Value;
                    string friendId = obj.Key;

                    Friend friend = LoadFriend(friendId, friendData);

                    // Remove if existed in community. Also, copy the community person's
                    // picture before removing
                    if (playerModel.community.ContainsKey(friendId))
                    {
                        friend.publicProfile.profilePicture = playerModel.community[friendId].publicProfile.profilePicture;
                        removeFriendSignal.Dispatch(friendId);
                    }

                    playerModel.friends.Add(friendId, friend);

                    refreshFriendsSignal.Dispatch();
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
        const string ATT_SKIP = "skip";

        public IPromise<BackendResult> Send(string op, string friendId, Action<object> onSuccess, int skip = 0, string fbToken = null)
		{
			this.errorCode = BackendResult.FRIENDS_OP_FAILED;
			this.onSuccess = onSuccess;

			new LogEventRequest().SetEventKey(SHORT_CODE)
				.SetEventAttribute(ATT_OP, op)
				.SetEventAttribute(ATT_FRIEND_ID, friendId)
                .SetEventAttribute(ATT_FBTOKEN, fbToken)
                .SetEventAttribute(ATT_SKIP, skip)
				.Send(OnRequestSuccess, OnRequestFailure);

			return promise;
		}
	}

	#endregion
}

