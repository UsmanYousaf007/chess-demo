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
        // Called by get init data and facebook auth commands
        private void ParseActiveChallenges(GSData data)
        {
            GSData activeChallengesData = data.GetGSData(GSBackendKeys.Match.ACTIVE_CHALLENGES);
            if (activeChallengesData == null)
                return;

            Dictionary<string, object> activeChallenges = GSJson.From(activeChallengesData.JSON) as Dictionary<string, object>;

            LogUtil.Log("Challenge count: " + activeChallenges.Count, "white");

            foreach (KeyValuePair<string, object> entry in activeChallenges)
            {
                GSData challengeData = activeChallengesData.GetGSData(entry.Key);
                GSData matchData = challengeData.GetGSData(GSBackendKeys.ChallengeData.MATCH_DATA_KEY);
                GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);
                ParseChallengeData(entry.Key, matchData, gameData, false);
            }
        }

        private void InitChallengeMessage(string challengeId, GSData scriptData)
        {
            // Because we preprocess messages upon GS connect, the player model
            // might not have set the player id. So we use the platform player id.
            if (playerModel.id == null)
            {
                playerModel.id = GS.GSPlatform.UserId;
            }
            LogUtil.Log("SCRIPT DATA: " + scriptData.JSON, "white");
            GSData challengeData = scriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            GSData matchData = challengeData.GetGSData(GSBackendKeys.ChallengeData.MATCH_DATA_KEY);
            GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);
            ParseChallengeData(challengeId, matchData, gameData, true);
        }

        private void ParseChallengeData(string challengeId, GSData matchData, GSData gameData, bool sourceIsMessage)
        {
            // The sourceIsMessage flag tells us whether this was an auto game generated message or whether we are parsing
            // via init data. This is an important distinction because init data is not allowed to override anything
            // inside matches or challenges if they have been pre-filled via an auto game generated message.
            // In addition message sources are allowed to only update matchinfo once.
            // In other words, leave if a source message has populated our model.
            if (matchInfoModel.matches.ContainsKey(challengeId) &&
                matchInfoModel.matches[challengeId].sourceIsMessage)
            {
                return;
            }

            // For the editor, the is also a case where active challenges are parsed twice because we manually authenticate
            // with gamesparks
            if (!sourceIsMessage && matchInfoModel.matches.ContainsKey(challengeId))
            {
                return;
            }

            MatchInfo matchInfo = matchInfoModel.CreateMatch(challengeId);
            matchInfo.sourceIsMessage = sourceIsMessage;

            string challengedId = matchData.GetString(GSBackendKeys.Match.CHALLENGED_ID);
            string challengerId = matchData.GetString(GSBackendKeys.Match.CHALLENGER_ID);
            string opponentId = (playerModel.id == challengerId) ? challengedId : challengerId;
            GSData opponentData = matchData.GetGSData(opponentId);
            GSData opponentProfile = opponentData.GetGSData(GSBackendKeys.ChallengeData.PROFILE);


            PublicProfile opponentPublicProfile = new PublicProfile();
            opponentPublicProfile.playerId = opponentId;
            opponentPublicProfile.name = opponentProfile.GetString(GSBackendKeys.ChallengeData.PROFILE_NAME);
            opponentPublicProfile.countryId = opponentProfile.GetString(GSBackendKeys.ChallengeData.PROFILE_COUNTRY_ID);
            opponentPublicProfile.eloScore = opponentProfile.GetInt(GSBackendKeys.ChallengeData.PROFILE_ELO_SCORE).Value;
            LogUtil.Log("Source Message:" + sourceIsMessage + " Parsed challenge vs " + opponentPublicProfile.name, "white");

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

            string shortCode = matchData.GetString(GSBackendKeys.Match.SHORT_CODE);

            if (shortCode == GSBackendKeys.Match.LONG_MATCH_SHORT_CODE)
            {
                matchInfo.gameStartTimeMilliseconds = matchData.GetLong(GSBackendKeys.GAME_START_TIME).Value;
            }

            matchInfoModel.matches[challengeId] = matchInfo;

            LoadGameData(gameData, challengeId);

            if (shortCode == GSBackendKeys.Match.QUICK_MATCH_SHORT_CODE)
            {   
                findMatchCompleteSignal.Dispatch(challengeId);
            }
            else if (shortCode == GSBackendKeys.Match.LONG_MATCH_SHORT_CODE)
            {
                MatchIdVO vo;
                vo.challengeId = challengeId;
                vo.opponentId = opponentId;
                longMatchReadySignal.Dispatch(vo);
            }
        }
    }
}
