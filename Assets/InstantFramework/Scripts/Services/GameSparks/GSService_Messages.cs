/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.Core;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public NotificationRecievedSignal notificationRecievedSignal { get; set; }
        [Inject] public SyncReconnectDataSignal syncReconnectDataSignal { get; set; }
        [Inject] public MatchAnalyticsSignal matchAnalyticsSignal { get; set; }

        public void AddChallengeListeners()
        {
            ChallengeStartedMessage.Listener += OnChallengeStartedMessage;
            ChallengeWonMessage.Listener += OnChallengeWonMessage;
            ChallengeLostMessage.Listener += OnChallengeLostMessage;
            ChallengeDrawnMessage.Listener += OnChallengeDrawnMessage;
            AddGameMessageListeners();
        }

        public void RemoveChallengeListeners()
        {
            ChallengeStartedMessage.Listener -= OnChallengeStartedMessage;
            ChallengeWonMessage.Listener -= OnChallengeWonMessage;
            ChallengeLostMessage.Listener -= OnChallengeLostMessage;
            ChallengeDrawnMessage.Listener -= OnChallengeDrawnMessage;
            RemoveGameMessageListeners();
        }

        public void AddMessageListeners()
        {
            ScriptMessage.Listener += OnScriptMessage;
            SessionTerminatedMessage.Listener += OnSessionTerminateMessage;
        }

        public void AddChatMessageListener()
        {
            ScriptMessage.Listener += OnChatScriptMessage;
        }

        private void OnScriptMessage(ScriptMessage message)
        {
            if (message.ExtCode == GSBackendKeys.NEW_FRIEND_MESSAGE)
            {
                string friendId = message.Data.GetString(GSBackendKeys.Friend.FRIEND_ID);
                newFriendSignal.Dispatch(friendId, false);
            }
            else if (message.ExtCode == GSBackendKeys.NEW_COMMUNITY_FRIEND_MESSAGE)
            {
                AddFriend(message.Data);
            }
            else if (message.ExtCode == GSBackendKeys.ONLINE_STATUS_FRIEND_MESSAGE)
            {
                string friendId = message.Data.GetString(GSBackendKeys.Friend.FRIEND_ID);
                bool isOnline = message.Data.GetBoolean(GSBackendKeys.Friend.IS_ONLINE).Value;

                TLUtils.LogUtil.LogNullValidation(friendId, "friendId");
                 
                if (friendId != null && playerModel.friends.ContainsKey(friendId))
                {
                    playerModel.friends[friendId].publicProfile.isOnline = isOnline;

                    PublicProfile publicProfile = playerModel.friends[friendId].publicProfile;
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
                    pvo.isPremium = publicProfile.isSubscriber;

                    updtateFriendOnlineStatusSignal.Dispatch(pvo);
                }
                else
                {
                    TLUtils.LogUtil.Log("OnScriptMessage::ONLINE_STATUS_FRIEND_MESSAGE friend not in list", "red");
                }
            }
            else if (message.ExtCode == GSBackendKeys.MATCH_WATCHDOG_PING_MESSAGE)
            {
                string currentTurnPlayerId = message.Data.GetString("currentTurnPlayerId");
                string challengerId = message.Data.GetString("challengerId");
                string challengedId = message.Data.GetString("challengedId");
                string challengeId = message.Data.GetString("challengeId");
                int moveCount = message.Data.GetInt("moveCount").Value;

                if (challengeId == matchInfoModel.activeChallengeId)
                {
                    MatchWatchdogPingAck(currentTurnPlayerId, challengerId, challengedId, challengeId, moveCount);
                }
                else
                {
                    LogUtil.Log("OnScriptMessage[MATCH_WATCHDOG_PING_MESSAGE]: challengeId=" + challengeId + " activeChallengeId=" + matchInfoModel.activeChallengeId, "yellow");
                }
            }
            else if (message.ExtCode == GSBackendKeys.CHALLENGE_ACCEPT_MESSAGE)
            {
                string challengeId = message.Data.GetString("challengeId");
                if (challengeId == matchInfoModel.activeChallengeId)
                {
                    challengeAcceptedSignal.Dispatch();
                }

                MatchAnalyticsVO matchAnalyticsVO = new MatchAnalyticsVO();
                matchAnalyticsVO.context = AnalyticsContext.accepted;
                matchAnalyticsVO.matchType = "classic";
                matchAnalyticsVO.eventID = AnalyticsEventId.match_find;
                Friend friend = playerModel.GetFriend(matchInfoModel.matches[challengeId].opponentPublicProfile.playerId);
                matchAnalyticsVO.friendType = friend.friendType;

                matchAnalyticsSignal.Dispatch(matchAnalyticsVO);

            }
            else if (message.ExtCode == GSBackendKeys.MATCH_WATCHDOG_OPPONENT_PINGED_MESSAGE)
            {
                string challengeId = message.Data.GetString("challengeId");
                if (challengeId == matchInfoModel.activeChallengeId)
                {
                    opponentPingedForConnectionSignal.Dispatch(false);
                }
            }
            else if (message.ExtCode == GSBackendKeys.MATCH_WATCHDOG_OPPONENT_ACKNOWLEDGED_MESSAGE)
            {
                string challengeId = message.Data.GetString("challengeId");
                if (challengeId == matchInfoModel.activeChallengeId)
                {
                    opponentPingedForConnectionSignal.Dispatch(true);
                }
            }
            else if (message.ExtCode == GSBackendKeys.AI_TAKE_TURN_FAILED_MESSAGE)
            {
                string challengeId = message.Data.GetString("challengeId");
                if (challengeId == matchInfoModel.activeChallengeId)
                {
                    syncReconnectDataSignal.Dispatch(matchInfoModel.activeChallengeId);
                }
                else
                {
                    LogUtil.Log("AiTakeTurnFailedMessage: challengeId=" + challengeId, "yellow");
                }
            }
            else if (message.ExtCode == "PushNotificationStandard")
            {
                NotificationVO notificationVO;
                notificationVO.title = "unassigned";
                notificationVO.body = "unassigned";

                notificationVO.isOpened = false;
                notificationVO.title = message.Data.ContainsKey("title") ? message.Data.GetString("title") : "unassigned";
                notificationVO.body = message.Data.ContainsKey("body") ? message.Data.GetString("body") : "unassigned";

                notificationVO.senderPlayerId = message.Data.ContainsKey("senderPlayerId") ? message.Data.GetString("senderPlayerId") : "undefined";
                notificationVO.challengeId = message.Data.ContainsKey("challengeId") ? message.Data.GetString("challengeId") : "undefined";
                notificationVO.matchGroup = message.Data.ContainsKey("matchGroup") ? message.Data.GetString("matchGroup") : "undefined";
                notificationVO.avatarId = message.Data.ContainsKey("avatarId") ? message.Data.GetString("avatarId") : "undefined";
                notificationVO.avaterBgColorId = message.Data.ContainsKey("avatarBgColorId") ? message.Data.GetString("avatarBgColorId") : "undefined";
                notificationVO.profilePicURL = message.Data.ContainsKey("profilePicURL") ? message.Data.GetString("profilePicURL") : "undefined";
                notificationVO.isPremium = message.Data.ContainsKey("isSubscriber") == true ? (bool)message.Data.GetBoolean("isSubscriber") : false;
                notificationVO.timeSent = message.Data.ContainsKey("creationTimestamp") == true ? long.Parse(message.Data.GetString("creationTimestamp")) : 0;
                notificationVO.actionCode = message.Data.ContainsKey("actionCode") == true ? message.Data.GetString("actionCode") : "undefined";

                if (notificationVO.title != "unassigned")
                {
                    notificationRecievedSignal.Dispatch(notificationVO);
                }
            }
        }

        private void OnChatScriptMessage(ScriptMessage message)
        {
            if (message.ExtCode == GSBackendKeys.Chat.CHAT_EXT_CODE)
            {
                ChatMessage msg = new ChatMessage();
                msg.recipientId = playerModel.id;
                msg.guid = message.Data.GetString(GSBackendKeys.Chat.GUID);

                GSData chatData = message.Data.GetGSData(GSBackendKeys.Chat.CHAT_DATA);
                msg.senderId = chatData.GetString(GSBackendKeys.Chat.SENDER_ID);
                msg.text = chatData.GetString(GSBackendKeys.Chat.TEXT);
                msg.timestamp = chatData.GetLong(GSBackendKeys.Chat.TIMESTAMP).Value;

                receiveChatMessageSignal.Dispatch(msg, false);

                NotificationVO notificationVO;
                notificationVO.title = message.Data.GetString("title");
                notificationVO.body = message.Data.GetString("body");
                notificationVO.senderPlayerId = msg.senderId;
                notificationVO.challengeId = message.Data.ContainsKey("challengeId") == true ? message.Data.GetString("challengeId") : "undefined";
                notificationVO.matchGroup = message.Data.ContainsKey("matchGroup") == true ? message.Data.GetString("matchGroup") : "undefined";
                notificationVO.avatarId = message.Data.ContainsKey("avatarId") == true ? message.Data.GetString("avatarId") : "undefined";
                notificationVO.avaterBgColorId = message.Data.ContainsKey("avatarBgColorId") == true ? message.Data.GetString("avatarBgColorId") : "undefined";
                notificationVO.profilePicURL = message.Data.ContainsKey("profilePicURL") == true ? message.Data.GetString("profilePicURL") : "undefined";
                notificationVO.isPremium = message.Data.ContainsKey("isSubscriber") == true ? (bool)message.Data.GetBoolean("isSubscriber") : false;
                notificationVO.timeSent = message.Data.ContainsKey("creationTimestamp") == true ? long.Parse(message.Data.GetString("creationTimestamp")) : 0;
                notificationVO.isOpened = false;
                notificationVO.actionCode = message.Data.ContainsKey("actionCode") == true ? message.Data.GetString("actionCode") : "undefined";

                notificationRecievedSignal.Dispatch(notificationVO);
            } 
        }

        private void OnSessionTerminateMessage(SessionTerminatedMessage message)
        {
            // Session terminated because this user authenticated on another device
            backendErrorSignal.Dispatch(BackendResult.SESSION_TERMINATED_ON_MULTIPLE_AUTH);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Handle challenge messages

        [System.Serializable]
        class challengeMessageData
        {
            public string botId;
            public bool isRanked;
            public string matchGroup;
        };

        private void OnChallengeStartedMessage(ChallengeStartedMessage message)
        {
            // Script data will be null for pending challenges
            if (message.ScriptData != null)
            {
                challengeMessageData data = new challengeMessageData();
                data = JsonUtility.FromJson<challengeMessageData>(message.Challenge.ChallengeMessage);

                bool forceStart = data.matchGroup == "RandomLong";

                GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
                ParseChallengeData(message.Challenge.ChallengeId, challengeData);
                HandleActiveNewMatch(message.Challenge.ChallengeId, forceStart);
            }
        }

        private void OnChallengeWonMessage(ChallengeWonMessage message)
        {
            UpdateEndGameStats(message.Challenge.ChallengeId, message.ScriptData);
            OnGameChallengeWonMessage(message);
        }

        private void OnChallengeLostMessage(ChallengeLostMessage message)
        {
            UpdateEndGameStats(message.Challenge.ChallengeId, message.ScriptData);
            OnGameChallengeLostMessage(message);
        }

        private void OnChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
            UpdateEndGameStats(message.Challenge.ChallengeId, message.ScriptData);
            OnGameChallengeDrawnMessage(message);
        }
    }
}
