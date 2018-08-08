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
        public void AddMessageListeners()
        {
            ScriptMessage.Listener += OnScriptMessage;
            ChallengeWonMessage.Listener += OnChallengeWonMessage;
            ChallengeLostMessage.Listener += OnChallengeLostMessage;
            ChallengeDrawnMessage.Listener += OnChallengeDrawnMessage;
            SessionTerminatedMessage.Listener += OnSessionTerminateMessage;

            // TODO: Eventually move to a game specific module
            AddGameMessageListeners();
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
            // Not listening for any messages right now
        }

        private void OnChallengeWonMessage(ChallengeWonMessage message)
        {
            if (!IsCurrentChallenge(message.Challenge.ChallengeId))
            {
                return;
            }

            GSData scriptData = message.ScriptData;
            EndGame(scriptData, matchInfoModel.opponentPublicProfile.id);
            OnGameChallengeWonMessage(message);
        }

        private void OnChallengeLostMessage(ChallengeLostMessage message)
        {
            if (!IsCurrentChallenge(message.Challenge.ChallengeId))
            {
                return;
            }

            GSData scriptData = message.ScriptData;
            EndGame(scriptData, matchInfoModel.opponentPublicProfile.id);
            OnGameChallengeLostMessage(message);
        }

        private void OnChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
            if (!IsCurrentChallenge(message.Challenge.ChallengeId))
            {
                return;
            }

            GSData scriptData = message.ScriptData;
            EndGame(scriptData, matchInfoModel.opponentPublicProfile.id);
            OnGameChallengeDrawnMessage(message);
        }

        private void EndGame(GSData data, string winnerId)
        {
            // Update player account details on game end.
            //GSData accountDetailsData = data.GetGSData(GSBackendKeys.ACCOUNT_DETAILS);
            //AccountDetailsResponse accountDetailsResponse = new AccountDetailsResponse(accountDetailsData);

            // Call the same method as for successful retrieval of account
            // details since we process the account details data in exactly the
            // same manner.
            //OnAccountDetailsSuccess(accountDetailsResponse);

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

        private bool IsCurrentChallenge(string challengeId)
        {
            return (challengeId == matchInfoModel.challengeId);
        }
    }
}
