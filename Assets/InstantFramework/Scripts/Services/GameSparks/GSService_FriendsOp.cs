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
        public IPromise<BackendResult> FriendsOpBlock(string friendId) { return new GSFriendsOpRequest(GetRequestContext()).Send("block", friendId, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpFriends() { return new GSFriendsOpRequest(GetRequestContext()).Send("friends", null, OnFriendOpSuccess); } 
        public IPromise<BackendResult> FriendsOpRefresh() { return new GSFriendsOpRequest(GetRequestContext()).Send("refresh", null, OnFriendOpSuccess); } 
        public IPromise<BackendResult> FriendsOpCommunity() { return new GSFriendsOpRequest(GetRequestContext()).Send("community", null, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpRegCommunity() { return new GSFriendsOpRequest(GetRequestContext()).Send("regcommunity", null, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpAdd(string friendId) { return new GSFriendsOpRequest(GetRequestContext()).Send("add", friendId, OnFriendOpSuccess, 0, facebookService.GetAccessToken()); }
        public IPromise<BackendResult> FriendsOpAddFavourite(string friendId) { return new GSFriendsOpRequest(GetRequestContext()).Send("addFavourite", friendId, OnFriendOpSuccess, 0, facebookService.GetAccessToken()); }
        public IPromise<BackendResult> FriendsOpInitialize() { return new GSFriendsOpRequest(GetRequestContext()).Send("initialize", null, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpRemove(string friendId, string opJson = null) { return new GSFriendsOpRequest(GetRequestContext()).Send("remove", friendId, OnFriendOpSuccess, 0, null, opJson); }
        public IPromise<BackendResult> FriendsOpSearch(string matchString, int skip) { return new GSFriendsOpRequest(GetRequestContext()).Send("search", matchString, OnFriendOpSuccess, skip); }
        public IPromise<BackendResult> FriendsOpStatus(string friendId) { return new GSFriendsOpRequest(GetRequestContext()).Send("status", friendId, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpUnblock(string friendId) { return new GSFriendsOpRequest(GetRequestContext()).Send("unblock", friendId, OnFriendOpSuccess); }
        public IPromise<BackendResult> FriendsOpBlocked(string friendId) { return new GSFriendsOpRequest(GetRequestContext()).Send("blocked", friendId, OnFriendOpSuccess); }

        private void OnFriendOpSuccess(object r, Action<object> a)
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
                //GSParser.LogFriends("search", playerModel.search);
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

            // Friend unblocked
            string unblockedId = response.ScriptData.GetString(GSBackendKeys.FriendsOp.UNBLOCK);
            if (unblockedId != null)
            {
                playerModel.blocked.Remove(unblockedId);
            }

            // Update friend status 
            GSData friendStatus = response.ScriptData.GetGSData(GSBackendKeys.FriendsOp.STATUS);
            if (friendStatus != null)
            {
                UpdateCommunityFriendStatus(friendStatus);
                //GSParser.LogFriends("status", playerModel.blocked);
            }

            // Populate chat if any
            GSData chatData = response.ScriptData.GetGSData(GSBackendKeys.CHAT);
            if (chatData != null)
            {
                FillChatModel(chatData);
            }
        }

        private void UpdateCommunityFriendStatus(GSData statusList)
        {
            foreach (KeyValuePair<string, object> obj in statusList.BaseData)
            {
                GSData player = (GSData)obj.Value;
                string playerId = obj.Key;
                bool isOnline = player.GetBoolean("isOnline").Value;
                string activity = player.GetString("activity");

                Friend friend = playerModel.GetFriend(playerId);
                if (friend == null)
                {
                    continue;
                }

                PublicProfile publicProfile = friend.publicProfile;

                ProfileVO pvo = new ProfileVO();
                pvo.playerPic = publicProfile.profilePicture;
                pvo.playerName = publicProfile.name;
                pvo.eloScore = publicProfile.eloScore;
                pvo.countryId = publicProfile.countryId;
                pvo.playerId = publicProfile.playerId;
                pvo.avatarColorId = publicProfile.avatarBgColorId;
                pvo.avatarId = publicProfile.avatarId;
                pvo.isOnline = isOnline;
                pvo.isActive = publicProfile.isActive;
                pvo.activity = activity;
                pvo.isPremium = publicProfile.isSubscriber;

                updtateFriendOnlineStatusSignal.Dispatch(pvo);
            }
        }

        private void AddFriend(GSData friendDict)
        {
            if (friendDict != null)
            {
                bool refreshFriends = false;

                foreach(KeyValuePair<string, object> obj in friendDict.BaseData)
                {
                    GSData friendData = (GSData)obj.Value;
                    string friendId = obj.Key;

                    TLUtils.LogUtil.LogNullValidation(friendId, "friendId");
                    
                    if (friendId != null)
                    {
                        //if already your friend
                        //only update the friend type
                        if (playerModel.friends.ContainsKey(friendId))
                        {
                            playerModel.friends[friendId].friendType = friendData.GetString(GSBackendKeys.Friend.TYPE);
                        }
                        else
                        {
                            Friend friend = LoadFriend(friendId, friendData);

                            // Remove if existed in community. Also, copy the community person's
                            // picture before removing

                            if (playerModel.community.ContainsKey(friendId))
                            {
                                friend.publicProfile.profilePicture = playerModel.community[friendId].publicProfile.profilePicture;
                                removeFriendSignal.Dispatch(friendId);
                            }

                            playerModel.friends.Add(friendId, friend);
                        }

                        refreshFriends = true;
                    }
                }

                if (refreshFriends)
                {
                    refreshFriendsSignal.Dispatch();
                    refreshCommunitySignal.Dispatch(false);
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
        const string ATT_JSON = "opJson";

        public GSFriendsOpRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string op, string friendId, Action<object, Action<object>> onSuccess, int skip = 0, string fbToken = null, string opJson = null)
		{
			this.errorCode = BackendResult.FRIENDS_OP_FAILED;
			this.onSuccess = onSuccess;

			new LogEventRequest().SetEventKey(SHORT_CODE)
				.SetEventAttribute(ATT_OP, op)
				.SetEventAttribute(ATT_FRIEND_ID, friendId)
                .SetEventAttribute(ATT_FBTOKEN, fbToken)
                .SetEventAttribute(ATT_SKIP, skip)
                .SetEventAttribute(ATT_JSON, opJson)
				.Send(OnRequestSuccess, OnRequestFailure);

			return promise;
		}
	}

	#endregion
}

