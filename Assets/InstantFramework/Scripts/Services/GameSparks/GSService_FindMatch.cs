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
            string activeAvatarsId = "unassigned";
            GSParser.GetActiveInventory(ref activeChessSkinsId, ref activeAvatarsId, activeInventoryData);
            //opponentPublicProfile.profilePicture = AvatarThumbsContainer.GetSprite(activeAvatarsId);

            GSData externalIds = opponentProfile.GetGSData(GSBackendKeys.MatchData.PROFILE_EXTERNAL_IDS);
            IDictionary<ExternalAuthType, ExternalAuthData> auths = GSBackendKeys.Auth.GetExternalAuthentications(externalIds);
            if (auths.ContainsKey(ExternalAuthType.FACEBOOK))
            {
                ExternalAuthData facebookAuthData = auths[ExternalAuthType.FACEBOOK];
                opponentPublicProfile.usingFacebookAuth = true;
                opponentPublicProfile.facebookAuthId = facebookAuthData.id;
            }

            matchInfoModel.challengeId = challengeId;
            matchInfoModel.opponentPublicProfile = opponentPublicProfile;
            matchInfoModel.botId = matchData.GetString(GSBackendKeys.MatchData.BOT_ID);
        }
    }

    #region REQUEST

    public class GSFindMatchRequest
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();
        private Action<ChallengeStartedMessage> onSuccess;

        public IPromise<BackendResult> Send(Action<ChallengeStartedMessage> onSuccess)
        {
            this.onSuccess = onSuccess;
            AddListeners();

            new LogEventRequest().SetEventKey("FindMatch")
                .Send((response) => {}, OnFailure);

            return promise;
        }

        private void OnFailure(LogEventResponse response)
        {
            DispatchResponse(BackendResult.MATCHMAKING_REQUEST_FAILED);
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
            onSuccess(message);
            DispatchResponse(BackendResult.SUCCESS);
        }

        private void DispatchResponse(BackendResult result)
        {  
            RemoveListeners();
            promise.Dispatch(result);
        }
    }

    #endregion
}
