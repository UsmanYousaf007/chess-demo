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
            LogUtil.Log("WE GOT MESSAGE:" + message.JSONString, "cyan");

            if (message.ExtCode == GSBackendKeys.NEW_FRIEND_MESSAGE)
            {
                string friendId = message.Data.GetString(GSBackendKeys.Friend.FRIEND_ID);
                newFriendSignal.Dispatch(friendId);
            }
        }

        private void OnChallengeWonMessage(ChallengeWonMessage message)
        {

            GSData scriptData = message.ScriptData;
            EndGame(scriptData);
            OnGameChallengeWonMessage(message);
        }

        private void OnChallengeLostMessage(ChallengeLostMessage message)
        {
            GSData scriptData = message.ScriptData;
            EndGame(scriptData);
            OnGameChallengeLostMessage(message);
        }

        private void OnChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
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

        private void OnChallengeStartedMessage(ChallengeStartedMessage message)
        {
            GSData challengeData = message.Challenge.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            GSData matchData = challengeData.GetGSData(GSBackendKeys.ChallengeData.MATCH_DATA_KEY);
            GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);

            var challenge = message.Challenge;
            string challengeId = challenge.ChallengeId;
            string challengerId = challenge.Challenger.Id;
            var enumerator = challenge.Challenged.GetEnumerator();
            bool hasChallengedPlayer = enumerator.MoveNext();

            Assertions.Assert(hasChallengedPlayer == true, "No challenged player has been returned from the backend!");

            string challengedId = enumerator.Current.Id;
            string opponentId = (playerModel.id == challengerId) ? challengedId : challengerId;
            GSData opponentData = matchData.GetGSData(opponentId);
            GSData opponentProfile = opponentData.GetGSData(GSBackendKeys.ChallengeData.PROFILE);

            MatchInfo matchInfo = matchInfoModel.CreateMatch(challengeId);
            matchInfo.isResuming = false;

            PublicProfile opponentPublicProfile = new PublicProfile();
            opponentPublicProfile.playerId = opponentId;
            opponentPublicProfile.name = opponentProfile.GetString(GSBackendKeys.ChallengeData.PROFILE_NAME);
            opponentPublicProfile.countryId = opponentProfile.GetString(GSBackendKeys.ChallengeData.PROFILE_COUNTRY_ID);
            opponentPublicProfile.eloScore = opponentProfile.GetInt(GSBackendKeys.ChallengeData.PROFILE_ELO_SCORE).Value;

            //IList<GSData> activeInventoryData = opponentProfile.GetGSDataList(GSBackendKeys.PLAYER_ACTIVE_INVENTORY);
            //string activeChessSkinsId = "unassigned";
            //GSParser.GetActiveInventory(ref activeChessSkinsId, activeInventoryData);

            GSData externalIds = opponentProfile.GetGSData(GSBackendKeys.ChallengeData.PROFILE_EXTERNAL_IDS);
            IDictionary<ExternalAuthType, ExternalAuth> auths = GSBackendKeys.Auth.GetExternalAuthentications(externalIds);
            if (auths.ContainsKey(ExternalAuthType.FACEBOOK))
            {
                ExternalAuth facebookAuthData = auths[ExternalAuthType.FACEBOOK];
                opponentPublicProfile.facebookUserId = facebookAuthData.id;
            }
                
            matchInfo.opponentPublicProfile = opponentPublicProfile;
            matchInfo.botId = matchData.GetString(GSBackendKeys.ChallengeData.BOT_ID);
            if (opponentData.ContainsKey(GSBackendKeys.ChallengeData.BOT_DIFFICULTY))
            {
                matchInfo.botDifficulty = opponentData.GetFloat(GSBackendKeys.ChallengeData.BOT_DIFFICULTY).Value;

                // Assign a random name to the bot
                int randomSuffix = UnityEngine.Random.Range(100, 10001);
                matchInfo.opponentPublicProfile.name = "Guest" + randomSuffix;
            }

            matchInfoModel.matches[challengeId] = matchInfo;

            // InitGame() is responsible for filling out all the game models
            // using the game specific data that comes as part of the response
            // in the ChallengeStartedMessage. Since Gamebet is not responsible
            // for any of the game models and reponse data this reponsibility
            // has to be delegated to the game side.
            InitGame(gameData, challengeId);

            if (challenge.ShortCode == GSBackendKeys.Match.QUICK_MATCH_SHORT_CODE)
            {   
                findMatchCompleteSignal.Dispatch(challengeId);
            }
            else if (challenge.ShortCode == GSBackendKeys.Match.LONG_MATCH_SHORT_CODE)
            {
                longMatchReadySignal.Dispatch(challengeId);
            }
        }
    }
}
