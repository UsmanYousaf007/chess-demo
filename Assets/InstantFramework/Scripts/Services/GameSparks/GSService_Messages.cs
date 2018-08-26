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
            LogUtil.Log("WE GOT MESSAGE:" + message.JSONString, "cyan");

            if (message.ExtCode == GSBackendKeys.NEW_FRIEND_MESSAGE)
            {
                string friendId = message.Data.GetString(GSBackendKeys.Friend.FRIEND_ID);
                newFriendSignal.Dispatch(friendId);
            }
        }

        private void OnChallengeWonMessage(ChallengeWonMessage message)
        {
            if (!IsCurrentChallenge(message.Challenge.ChallengeId))
            {
                return;
            }

            GSData scriptData = message.ScriptData;
            EndGame(scriptData);
            OnGameChallengeWonMessage(message);
        }

        private void OnChallengeLostMessage(ChallengeLostMessage message)
        {
            if (!IsCurrentChallenge(message.Challenge.ChallengeId))
            {
                return;
            }

            GSData scriptData = message.ScriptData;
            EndGame(scriptData);
            OnGameChallengeLostMessage(message);
        }

        private void OnChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
            if (!IsCurrentChallenge(message.Challenge.ChallengeId))
            {
                return;
            }

            GSData scriptData = message.ScriptData;
            EndGame(scriptData);
            OnGameChallengeDrawnMessage(message);
        }

        private void EndGame(GSData data)
        {
            // Update player account details on game end.
            GSData updatedStatsData = data.GetGSData(GSBackendKeys.UPDATED_STATS);
            playerModel.eloScore = updatedStatsData.GetInt(GSBackendKeys.ELO_SCORE).Value;
            playerModel.totalGamesWon = updatedStatsData.GetInt(GSBackendKeys.GAMES_WON).Value;
            playerModel.totalGamesLost = updatedStatsData.GetInt(GSBackendKeys.GAMES_LOST).Value;
        }

        private void OnSessionTerminateMessage(SessionTerminatedMessage message)
        {
            // Session terminated because this user authenticated on another device
            backendErrorSignal.Dispatch(BackendResult.SESSION_TERMINATED_ON_MULTIPLE_AUTH);
        }

        private bool IsCurrentChallenge(string challengeId)
        {
            return (challengeId == matchInfoModel.activeMatch.challengeId);
        }
    }
}
