/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-04 13:41:42 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public IPromise<BackendResult> FindMatch(string groupId)
        {
            return new GSFindMatchRequest().Send(groupId, OnFindMatchSuccess);
        }

        public IPromise<BackendResult> GetGameStartTime(string challengeId)
        {
            return new GSGetGameStartTimeRequest().Send(challengeId, OnGetGameStartTimeSuccess);
        }

        private void OnFindMatchSuccess(ChallengeStartedMessage message)
        {
            GSData matchData = message.ScriptData.GetGSData(GSBackendKeys.MatchData.KEY);
            GSData gameData = message.ScriptData.GetGSData(GSBackendKeys.GAME_DATA);

            var challenge = message.Challenge;

            string challengeId = challenge.ChallengeId;
            string challengerId = challenge.Challenger.Id;

            var enumerator = challenge.Challenged.GetEnumerator();
            bool hasChallengedPlayer = enumerator.MoveNext();

            Assertions.Assert(hasChallengedPlayer == true, "No challenged player has been returned from the backend!");

            string challengedId = enumerator.Current.Id;

            // Set up resume specific data. Specify not resuming.
            matchInfoModel.isResuming = false;

            // Set up common data
            LoadData(matchData, gameData, challengeId, challengerId, challengedId);

            // InitGame() is responsible for filling out all the game models
            // using the game specific data that comes as part of the response
            // in the ChallengeStartedMessage. Since Gamebet is not responsible
            // for any of the game models and reponse data this reponsibility
            // has to be delegated to the game side.
            InitGame(gameData);
        }

        private void CheckAndHandleMatchResume(LogEventResponse response)
        {
            GSData resumeData = response.ScriptData.GetGSData(GSBackendKeys.ResumeData.KEY);

            if (resumeData == null)
            {
                matchInfoModel.isResuming = false;
            }
            else
            {
                GSData matchData = resumeData.GetGSData(GSBackendKeys.MatchData.KEY);
                GSData gameData = resumeData.GetGSData(GSBackendKeys.GAME_DATA);

                string challengeId = resumeData.GetString(GSBackendKeys.CHALLENGE_ID);
                string challengerId = resumeData.GetString(GSBackendKeys.CHALLENGER_ID);
                string challengedId = resumeData.GetString(GSBackendKeys.CHALLENGED_ID);

                // Set up resume specific data. Specify
                matchInfoModel.isResuming = true;
                matchInfoModel.gameStartTimeMilliseconds = matchData.GetLong(GSBackendKeys.MatchData.GAME_START_TIME).Value;
                matchInfoModel.roomId = matchData.GetString(GSBackendKeys.MatchData.ROOM_ID);

                // Set up common data
                LoadData(matchData, gameData, challengeId, challengerId, challengedId);

                InitGameForResume(resumeData);
            }
        }

        private void OnGetGameStartTimeSuccess(ScriptMessage message)
        {
            matchInfoModel.gameStartTimeMilliseconds = message.Data.GetLong(GSBackendKeys.GAME_START_TIME).Value;
        }

        // TODO(mubeeniqbal): Remove gameData if it is not required in the
        // parameters for any purpose.
        private void LoadData(GSData matchData,
                              GSData gameData,
                              string challengeId,
                              string challengerId,
                              string challengedId)
        {
            string opponentId = (playerModel.id == challengerId) ? challengedId : challengerId;
            GSData opponentData = matchData.GetGSData(opponentId);
            GSData opponentProfile = opponentData.GetGSData(GSBackendKeys.MatchData.PROFILE);

            PublicProfile opponentPublicProfile;
            opponentPublicProfile.id = opponentId;
            opponentPublicProfile.name = opponentProfile.GetString(GSBackendKeys.MatchData.PROFILE_NAME);
            opponentPublicProfile.countryId = opponentProfile.GetString(GSBackendKeys.MatchData.PROFILE_COUNTRY_ID);
            opponentPublicProfile.level = opponentProfile.GetInt(GSBackendKeys.MatchData.PROFILE_LEVEL).Value;
            opponentPublicProfile.leagueId = opponentProfile.GetString(GSBackendKeys.MatchData.PROFILE_LEAGUE_ID);
            opponentPublicProfile.eloDivision = opponentProfile.GetString(GSBackendKeys.MatchData.PROFILE_ELO_DIVISION);
            opponentPublicProfile.eloScore = opponentProfile.GetInt(GSBackendKeys.MatchData.PROFILE_ELO_SCORE).Value;

            IList<GSData> activeInventoryData = opponentProfile.GetGSDataList(GSBackendKeys.PLAYER_ACTIVE_INVENTORY);
            string activeChessSkinsId = "unassigned";
            string activeAvatarsId = "unassigned";
            string activeAvatarsBorderId = "unassigned";         
            GSParser.GetActiveInventory(ref activeChessSkinsId, ref activeAvatarsId, ref activeAvatarsBorderId, activeInventoryData);
            opponentPublicProfile.profilePicture = AvatarThumbsContainer.container.GetThumb(activeAvatarsId).thumbnail;
            opponentPublicProfile.profilePictureBorder = AvatarBorderThumbsContainer.container.GetThumb(activeAvatarsBorderId).thumbnail;
         
            GSData roomRecords = opponentProfile.GetGSData(GSBackendKeys.MatchData.ROOM_RECORDS);
            opponentPublicProfile.roomRecords = GSParser.ParseRoomRecords(roomRecords, roomSettingsModel.settings);

            GSData externalIds = opponentProfile.GetGSData(GSBackendKeys.MatchData.PROFILE_EXTERNAL_IDS);
            opponentPublicProfile.externalAuthentications = GSBackendKeys.Auth.GetExternalAuthentications(externalIds);

            matchInfoModel.challengeId = challengeId;
            matchInfoModel.opponentPublicProfile = opponentPublicProfile;
            matchInfoModel.botId = matchData.GetString(GSBackendKeys.MatchData.BOT_ID);
        }
    }
}
