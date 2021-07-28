/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using System.Collections.Generic;
using strange.extensions.promise.api;
using SimpleJson2;


namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        // Note: This is not used as of version 6.5.21 (championship)
        public IPromise<BackendResult> TournamentsOpJoin(string tournamentShortCode, int score)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("tournamentShortCode", tournamentShortCode);
            jsonObj.Add("score", score);

            return new GSTournamentsOpRequest(GetRequestContext()).Send("join", OnTournamentsOpSuccess, jsonObj.ToString());
        }

        // Note: This is not used as of version 6.5.21 (championship)
        public IPromise<BackendResult> TournamentsOpGetJoinedTournaments()
        {
            return new GSTournamentsOpRequest(GetRequestContext()).Send("getJoinedTournaments", OnTournamentsOpSuccess);
        }

        // Note: This is not used as of version 6.5.21 (championship)
        public IPromise<BackendResult> TournamentsOpGetLiveTournaments()
        {
            return new GSTournamentsOpRequest(GetRequestContext()).Send("getLiveTournaments", OnTournamentsOpSuccess);
        }

        // Note: This is not used as of version 6.5.21 (championship)
        public IPromise<BackendResult> TournamentsOpGetAllTournaments()
        {
            return new GSTournamentsOpRequest(GetRequestContext()).Send("getAllTournaments", OnTournamentsOpSuccess);
        }

        public IPromise<BackendResult> TournamentsOpGetLeaderboard(string tournamentId, bool update = true)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("tournamentId", tournamentId);
            jsonObj.Add("update", update);

            return new GSTournamentsOpRequest(GetRequestContext()).Send("getTournamentLeaderboard", OnTournamentsOpSuccess, jsonObj.ToString());
        }

        // Note: This is not used as of version 6.5.21 (championship)
        public IPromise<BackendResult> TournamentsOpGetLiveRewards(string tournamentShortCode)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("tournamentShortCode", tournamentShortCode);

            return new GSTournamentsOpRequest(GetRequestContext()).Send("getLiveTournamentRewards", OnTournamentsOpSuccess, jsonObj.ToString());
        }

        public IPromise<BackendResult> TournamentsOpUpdateTournaments(string endedTournamentId = null)
        {
            JsonObject jsonObj = null;
            if (string.IsNullOrEmpty(endedTournamentId) == false)
            {
                jsonObj = new JsonObject();
                jsonObj.Add("concludedTournamentId", endedTournamentId);
            }

            return new GSTournamentsOpRequest(GetRequestContext()).Send("updateTournaments", OnTournamentsOpSuccess, jsonObj != null ? jsonObj.ToString() : null);
        }

        private void OnTournamentsOpSuccess(object r, Action<object> a)
        {
            LogEventResponse response = (LogEventResponse)r;

            if (response.ScriptData == null)
            {
                return;
            }

            GSData error = response.ScriptData.GetGSData(GSBackendKeys.TournamentsOp.ERROR);
            if (error != null)
            {
                // We can dispatch error signal here
                return;
            }

            var playerModelUpdated = false;

            if (response.ScriptData.ContainsKey(GSBackendKeys.PlayerDetails.LEAGUE))
            {
                playerModel.league = response.ScriptData.GetInt(GSBackendKeys.PlayerDetails.LEAGUE).Value;
                playerModelUpdated = true;
            }

            if (response.ScriptData.ContainsKey(GSBackendKeys.PlayerDetails.TROPHIES))
            {
                playerModel.trophies = response.ScriptData.GetInt(GSBackendKeys.PlayerDetails.TROPHIES).Value;
                playerModelUpdated = true;
            }

            if (response.ScriptData.ContainsKey(GSBackendKeys.PlayerDetails.TROPHIES2))
            {
                playerModel.trophies2 = response.ScriptData.GetInt(GSBackendKeys.PlayerDetails.TROPHIES2).Value;
                playerModelUpdated = true;
            }

            if (playerModelUpdated)
            {
                playerModelUpdatedSignal.Dispatch(playerModel);
            }

            if (response.ScriptData.ContainsKey("inbox"))
            {
                GSData inboxData = response.ScriptData.GetGSData("inbox");
                PopulateInboxModel(inboxData);
            }

            if (response.ScriptData.ContainsKey("inboxCount"))
            {
                int inboxMessageCount = response.ScriptData.GetInt("inboxCount").Value;
                inboxModel.inboxMessageCount = inboxMessageCount;
                updateInboxMessageCountViewSignal.Dispatch(inboxModel.inboxMessageCount);
            }

            GSData joinedTournaments = response.ScriptData.GetGSData(GSBackendKeys.TournamentsOp.TOURNAMENTS);
            if (joinedTournaments != null)
            {
                FillJoinedTournaments(joinedTournaments);
                tournamentsModel.lastFetchedTime = DateTime.UtcNow;
            }

            List<GSData> liveTournaments = response.ScriptData.GetGSDataList(GSBackendKeys.TournamentsOp.LIVE_TOURNAMENTS);
            if (liveTournaments != null)
            {
                FillLiveTournaments(liveTournaments);
                tournamentsModel.lastFetchedTime = DateTime.UtcNow;
            }

            GSData tournament = response.ScriptData.GetGSData(GSBackendKeys.TournamentsOp.TOURNAMENT);
            if (tournament != null)
            {
                string tournamentId = GSParser.GetSafeString(tournament, GSBackendKeys.Tournament.TOURNAMENT_ID);
                GSData tournamentGSData = tournament.GetGSData(GSBackendKeys.Tournament.TOURNAMENT_KEY);

                List <JoinedTournamentData> joinedTournamentsList = tournamentsModel.joinedTournaments;
                bool tournamentExists = false;
                for (int i = 0; i < joinedTournamentsList.Count; i++)
                {
                    if (joinedTournamentsList[i].id == tournamentId)
                    {
                        joinedTournamentsList[i] = ParseJoinedTournament(tournamentGSData, joinedTournamentsList[i].id, joinedTournamentsList[i]);
                        joinedTournamentsList[i].lastFetchedTimeUTCSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        tournamentExists = true;
                    }
                }

                if (!tournamentExists)
                {
                    JoinedTournamentData joinedTournament = ParseJoinedTournament(tournamentGSData, tournamentId);
                    joinedTournament.lastFetchedTimeUTCSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    tournamentsModel.joinedTournaments.Add(joinedTournament);
                }
            }

            GSData liveTournamentGSData = response.ScriptData.GetGSData(GSBackendKeys.TournamentsOp.LIVE_TOURNAMENT);
            if (liveTournamentGSData != null)
            {
                string shortCode = GSParser.GetSafeString(liveTournamentGSData, GSBackendKeys.Tournament.SHORT_CODE);
                LiveTournamentData openTournament = null;
                LiveTournamentData upcomingTournament = null;
                LiveTournamentData liveTournamentData = null;
                if (shortCode != null)
                {
                    openTournament = tournamentsModel.GetOpenTournament(shortCode);
                    upcomingTournament = tournamentsModel.GetOpenTournament(shortCode);

                    liveTournamentData = openTournament != null ? openTournament : upcomingTournament;
                }

                LiveTournamentData newLiveTournament = ParseLiveTournament(liveTournamentGSData, liveTournamentData);
            
                newLiveTournament.lastFetchedTimeUTCSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                tournamentsModel.SetOpenTournament(newLiveTournament);
            }
        }
    }

    #region REQUEST

    public class GSTournamentsOpRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "TournamentsOp";
        const string ATT_OP = "op";
        const string ATT_JSON = "opJson";

        public GSTournamentsOpRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string op, Action<object, Action<object>> onSuccess, string opJson = null)
        {
            this.errorCode = BackendResult.TOURNAMENTS_OP_FAILED;
            this.onSuccess = onSuccess;

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_OP, op)
                .SetEventAttribute(ATT_JSON, opJson)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}

