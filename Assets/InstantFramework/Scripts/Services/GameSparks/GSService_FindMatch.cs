/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using strange.extensions.promise.impl;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using TurboLabz.TLUtils;
using GameSparks.Core;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> FindMatch()
        {
            return new GSFindMatchRequest().Send(OnFindMatchSuccess);
        }

        private void OnFindMatchSuccess(object m)
        {
            ChallengeStartedMessage message = (ChallengeStartedMessage)m;
            GSData matchData = message.ScriptData.GetGSData(GSBackendKeys.MatchData.KEY);
            GSData gameData = message.ScriptData.GetGSData(GSBackendKeys.GAME_DATA);

            var challenge = message.Challenge;
            string challengeId = challenge.ChallengeId;
            string challengerId = challenge.Challenger.Id;
            var enumerator = challenge.Challenged.GetEnumerator();
            bool hasChallengedPlayer = enumerator.MoveNext();

            Assertions.Assert(hasChallengedPlayer == true, "No challenged player has been returned from the backend!");

            string challengedId = enumerator.Current.Id;
            matchInfoModel.isResuming = false;

            string opponentId = (playerModel.id == challengerId) ? challengedId : challengerId;
            GSData opponentData = matchData.GetGSData(opponentId);
            GSData opponentProfile = opponentData.GetGSData(GSBackendKeys.MatchData.PROFILE);

            PublicProfile opponentPublicProfile = new PublicProfile();
            opponentPublicProfile.id = opponentId;
            opponentPublicProfile.name = opponentProfile.GetString(GSBackendKeys.MatchData.PROFILE_NAME);
            opponentPublicProfile.countryId = opponentProfile.GetString(GSBackendKeys.MatchData.PROFILE_COUNTRY_ID);
            opponentPublicProfile.eloScore = opponentProfile.GetInt(GSBackendKeys.MatchData.PROFILE_ELO_SCORE).Value;

            IList<GSData> activeInventoryData = opponentProfile.GetGSDataList(GSBackendKeys.PLAYER_ACTIVE_INVENTORY);
            string activeChessSkinsId = "unassigned";
            GSParser.GetActiveInventory(ref activeChessSkinsId, activeInventoryData);

            GSData externalIds = opponentProfile.GetGSData(GSBackendKeys.MatchData.PROFILE_EXTERNAL_IDS);
            IDictionary<ExternalAuthType, ExternalAuth> auths = GSBackendKeys.Auth.GetExternalAuthentications(externalIds);
            if (auths.ContainsKey(ExternalAuthType.FACEBOOK))
            {
                ExternalAuth facebookAuthData = auths[ExternalAuthType.FACEBOOK];
                opponentPublicProfile.usingFacebookAuth = true;
                opponentPublicProfile.facebookAuthId = facebookAuthData.id;
            }

            matchInfoModel.challengeId = challengeId;
            matchInfoModel.opponentPublicProfile = opponentPublicProfile;
            matchInfoModel.botId = matchData.GetString(GSBackendKeys.MatchData.BOT_ID);

            // InitGame() is responsible for filling out all the game models
            // using the game specific data that comes as part of the response
            // in the ChallengeStartedMessage. Since Gamebet is not responsible
            // for any of the game models and reponse data this reponsibility
            // has to be delegated to the game side.
            InitGame(gameData);
        }
    }

    #region REQUEST

    public class GSFindMatchRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "FindMatch";

        public IPromise<BackendResult> Send(Action<object> onSuccess)
        {
            this.onSuccess = onSuccess;
            this.errorCode = BackendResult.MATCHMAKING_REQUEST_FAILED;
            AddListeners();

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .Send(null, OnRequestFailure);

            return promise;
        }

        private void AddListeners()
        {
            ChallengeStartedMessage.Listener += OnChallengeStarted;
        }

        private void RemoveListeners()
        {
            ChallengeStartedMessage.Listener -= OnChallengeStarted;
        }

        private void OnChallengeStarted(ChallengeStartedMessage message) 
        {
            this.OnRequestSuccess(message);
        }
    }

    #endregion
}
