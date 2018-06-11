/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public void AddMessageListeners()
        {
            ScriptMessage.Listener += OnScriptMessage;
            ChallengeWonMessage.Listener += OnChallengeWonMessage;
            ChallengeLostMessage.Listener += OnChallengeLostMessage;
            ChallengeDrawnMessage.Listener += OnChallengeDrawnMessage;
            SessionTerminatedMessage.Listener += OnSessionTerminateMessage;

            // TODO: Eventually move to a game specific module
            // AddGameMessageListeners();
        }

        private void RemoveMessageListeners()
        {
            // TODO: Eventually move to a game specific module
            //RemoveGameMessageListeners();

            ChallengeDrawnMessage.Listener -= OnChallengeDrawnMessage;
            ChallengeLostMessage.Listener -= OnChallengeLostMessage;
            ChallengeWonMessage.Listener -= OnChallengeWonMessage;
            ScriptMessage.Listener -= OnScriptMessage;
            SessionTerminatedMessage.Listener -= OnSessionTerminateMessage;
        }

        private void OnScriptMessage(ScriptMessage message)
        {
            if (message.ExtCode == GSBackendKeys.OPPONENT_DISCONNECTED_MESSAGE)
            {
                if (!IsCurrentChallenge(message.Data.GetString(GSBackendKeys.CHALLENGE_ID)))
                {
                    return;
                }

                // TODO: Eventually move to a game specific module or convert to signal
                // OnOpponentDisconnect();
            }
            else if (message.ExtCode == GSBackendKeys.OPPONENT_RECONNECTED_MESSAGE)
            {
                if (!IsCurrentChallenge(message.Data.GetString(GSBackendKeys.CHALLENGE_ID)))
                {
                    return;
                }

                // TODO: Eventually move to a game specific module or convert to signal
                // OnOpponentReconnect();

            }
        }

        private bool IsCurrentChallenge(string challengeId)
        {
            return (challengeId == matchInfoModel.challengeId);
        }

        private void OnChallengeWonMessage(ChallengeWonMessage message)
        {
            GSData scriptData = message.ScriptData;
            EndGame(scriptData, matchInfoModel.opponentPublicProfile.id);
        }

        private void OnChallengeLostMessage(ChallengeLostMessage message)
        {
            GSData scriptData = message.ScriptData;
            EndGame(scriptData, matchInfoModel.opponentPublicProfile.id);
        }

        private void OnChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
            GSData scriptData = message.ScriptData;
            EndGame(scriptData, matchInfoModel.opponentPublicProfile.id);
        }

        private void EndGame(GSData data, string winnerId)
        {
            // Update player account details on game end.
            GSData accountDetailsData = data.GetGSData(GSBackendKeys.ACCOUNT_DETAILS);
            AccountDetailsResponse accountDetailsResponse = new AccountDetailsResponse(accountDetailsData);

            // Call the same method as for successful retrieval of account
            // details since we process the account details data in exactly the
            // same manner.
            OnAccountDetailsSuccess(accountDetailsResponse);

            // Opponent public profile elo update
            PublicProfile opponentPublicProfile = matchInfoModel.opponentPublicProfile;

            opponentPublicProfile.eloScore = data.GetInt(GSBackendKeys.MatchData.OPPONENT_ELO_SCORE).Value;

            matchInfoModel.opponentPublicProfile = opponentPublicProfile;
        }

        private void OnSessionTerminateMessage(SessionTerminatedMessage message)
        {
            // Session terminated because this user authenticated on another device
            backendErrorSignal.Dispatch(BackendResult.SESSION_TERMINATED_ON_MULTIPLE_AUTH);
        }
    }
}
