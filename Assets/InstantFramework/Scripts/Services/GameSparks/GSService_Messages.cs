/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.Core;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public void AddChallengeListeners()
        {
            ChallengeStartedMessage.Listener += OnChallengeStartedMessage;
            ChallengeWonMessage.Listener += OnChallengeWonMessage;
            ChallengeLostMessage.Listener += OnChallengeLostMessage;
            ChallengeDrawnMessage.Listener += OnChallengeDrawnMessage;
            AddGameMessageListeners();
        }

        public void AddMessageListeners()
        {
            ScriptMessage.Listener += OnScriptMessage;
            SessionTerminatedMessage.Listener += OnSessionTerminateMessage;
        }

        private void OnScriptMessage(ScriptMessage message)
        {
            if (message.ExtCode == GSBackendKeys.NEW_FRIEND_MESSAGE)
            {
                string friendId = message.Data.GetString(GSBackendKeys.Friend.FRIEND_ID);
                newFriendSignal.Dispatch(friendId);
            }
            else if (message.ExtCode == GSBackendKeys.NEW_COMMUNITY_FRIEND_MESSAGE)
            {
                AddFriend(message.Data);
            }
            else if (message.ExtCode == GSBackendKeys.ONLINE_STATUS_FRIEND_MESSAGE)
            {
                string friendId = message.Data.GetString(GSBackendKeys.Friend.FRIEND_ID);
                bool isOnline = message.Data.GetBoolean(GSBackendKeys.Friend.IS_ONLINE).Value;

                if (playerModel.friends.ContainsKey(friendId))
                {
                    playerModel.friends[friendId].publicProfile.isOnline = isOnline;
                }

                updtateFriendOnlineStatusSignal.Dispatch(friendId, isOnline);
            }
            else if (message.ExtCode == GSBackendKeys.Chat.CHAT_EXT_CODE)
            {
                ChatMessage msg;
                msg.senderId = message.Data.GetString(GSBackendKeys.Chat.SENDER_ID);
                msg.recipientId = playerModel.id;
                msg.text = message.Data.GetString(GSBackendKeys.Chat.TEXT);
                msg.timestamp = message.Data.GetLong(GSBackendKeys.Chat.TIMESTAMP).Value;

                receiveChatMessageSignal.Dispatch(msg);
            }
        }

        private void OnSessionTerminateMessage(SessionTerminatedMessage message)
        {
            // Session terminated because this user authenticated on another device
            backendErrorSignal.Dispatch(BackendResult.SESSION_TERMINATED_ON_MULTIPLE_AUTH);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Handle challenge messages

        private void OnChallengeStartedMessage(ChallengeStartedMessage message)
        {
            // Script data will be null for pending challenges
            if (message.ScriptData != null)
            {
                GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
                ParseChallengeData(message.Challenge.ChallengeId, challengeData);
                HandleActiveNewMatch(message.Challenge.ChallengeId);
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
