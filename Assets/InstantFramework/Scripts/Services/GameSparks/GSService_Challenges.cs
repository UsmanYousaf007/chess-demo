/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;

using TurboLabz.TLUtils;
using System;
using GameSparks.Api.Requests;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }

        // Called by get init data and facebook auth commands
        private void ParseActiveChallenges(GSData data)
        {
            GSData activeChallengesData = data.GetGSData(GSBackendKeys.Match.ACTIVE_CHALLENGES);
            if (activeChallengesData == null)
                return;

            Dictionary<string, object> activeChallenges = GSJson.From(activeChallengesData.JSON) as Dictionary<string, object>;

            foreach (KeyValuePair<string, object> entry in activeChallenges)
            {
                GSData challengeData = activeChallengesData.GetGSData(entry.Key);
                ParseChallengeData(entry.Key, challengeData);
            }
        }

        private void ParseChallengeData(string challengeId, GSData challengeData)
        {
            GSData matchData = challengeData.GetGSData(GSBackendKeys.ChallengeData.MATCH_DATA_KEY);
            GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);
            if (playerModel.id == null) playerModel.id = GS.GSPlatform.UserId; // Player id is not set during backlog messages

            if (!matchInfoModel.matches.ContainsKey(challengeId))
            {
                SetupMatch(challengeId, GSData matchData, GSData gameData);
                SetupGame(challengeId, gameData);
            }








            LoadGameData(gameData, challengeId);

            if (shortCode == GSBackendKeys.Match.QUICK_MATCH_SHORT_CODE)
            {   
                findMatchCompleteSignal.Dispatch(challengeId);
            }
            else if (shortCode == GSBackendKeys.Match.LONG_MATCH_SHORT_CODE)
            {
                if (opponentId == matchInfoModel.activeLongMatchOpponentId &&
                    !playerModel.blocked.ContainsKey(opponentId))
                {
                    startLongMatchSignal.Dispatch(challengeId);
                }
            }

            updateFriendBarSignal.Dispatch(opponentId);



            //////////////////////////////
            /// SETUP MATCH
            //////////////////////////////





            //////////////////////////////
            /// UPDATE MATCH
            //////////////////////////////
            matchInfo.acceptStatus = matchData.GetString(GSBackendKeys.Match.ACCEPT_STATUS_KEY);



        }

        private void SetupMatch(string challengeId, GSData matchData, GSData gameData)
        {
            ///////////////////////////////////////////////////////////////////////////////////////
            // Create new match
            MatchInfo matchInfo = matchInfoModel.CreateMatch(challengeId);

            ///////////////////////////////////////////////////////////////////////////////////////
            // Initialize fixed match data
            string shortCode = matchData.GetString(GSBackendKeys.Match.SHORT_CODE);
            matchInfo.isLongPlay = (shortCode == GSBackendKeys.Match.LONG_MATCH_SHORT_CODE) ? true : false;
            matchInfo.challengedId = matchData.GetString(GSBackendKeys.Match.CHALLENGED_ID);
            matchInfo.challengerId = matchData.GetString(GSBackendKeys.Match.CHALLENGER_ID);
            if (shortCode == GSBackendKeys.Match.LONG_MATCH_SHORT_CODE)
            {
                matchInfo.gameStartTimeMilliseconds = matchData.GetLong(GSBackendKeys.GAME_START_TIME).Value;
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // Initialize opponent profile
            string opponentId = (playerModel.id == matchInfo.challengerId) ? matchInfo.challengedId : matchInfo.challengerId;
            GSData opponentData = matchData.GetGSData(opponentId);
            GSData opponentProfile = opponentData.GetGSData(GSBackendKeys.ChallengeData.PROFILE);
            PublicProfile opponentPublicProfile = new PublicProfile();
            opponentPublicProfile.playerId = opponentId;
            opponentPublicProfile.name = opponentProfile.GetString(GSBackendKeys.ChallengeData.PROFILE_NAME);
            opponentPublicProfile.countryId = opponentProfile.GetString(GSBackendKeys.ChallengeData.PROFILE_COUNTRY_ID);
            opponentPublicProfile.eloScore = opponentProfile.GetInt(GSBackendKeys.ChallengeData.PROFILE_ELO_SCORE).Value;
            GSData externalIds = opponentProfile.GetGSData(GSBackendKeys.ChallengeData.PROFILE_EXTERNAL_IDS);
            IDictionary<ExternalAuthType, ExternalAuth> auths = GSBackendKeys.Auth.GetExternalAuthentications(externalIds);
            if (auths.ContainsKey(ExternalAuthType.FACEBOOK))
            {
                ExternalAuth facebookAuthData = auths[ExternalAuthType.FACEBOOK];
                opponentPublicProfile.facebookUserId = facebookAuthData.id;
            }
            matchInfo.opponentPublicProfile = opponentPublicProfile;

            ///////////////////////////////////////////////////////////////////////////////////////
            // Initialize bots
            matchInfo.botId = matchData.GetString(GSBackendKeys.ChallengeData.BOT_ID);
            if (opponentData.ContainsKey(GSBackendKeys.ChallengeData.BOT_DIFFICULTY))
            {
                matchInfo.botDifficulty = opponentData.GetFloat(GSBackendKeys.ChallengeData.BOT_DIFFICULTY).Value;

                // Assign a random name to the bot
                int randomSuffix = UnityEngine.Random.Range(100, 10001);
                matchInfo.opponentPublicProfile.name = "Guest" + randomSuffix;
            }
        }
    }
}
