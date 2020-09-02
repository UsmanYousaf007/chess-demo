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
        public IPromise<BackendResult> TournamentsOpJoin(string tournamentShortCode, int score)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("tournamentShortCode", tournamentShortCode);
            jsonObj.Add("score", score);

            return new GSTournamentsOpRequest(GetRequestContext()).Send("join", OnTournamentsOpSuccess, jsonObj.ToString());
        }

        public IPromise<BackendResult> TournamentsOpGetJoinedTournaments()
        {
            return new GSTournamentsOpRequest(GetRequestContext()).Send("getJoinedTournaments", OnTournamentsOpSuccess);
        }

        public IPromise<BackendResult> TournamentsOpGetLiveTournaments()
        {
            return new GSTournamentsOpRequest(GetRequestContext()).Send("getLiveTournaments", OnTournamentsOpSuccess);
        }

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

        public IPromise<BackendResult> TournamentsOpGetLiveRewards(string tournamentShortCode)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("tournamentShortCode", tournamentShortCode);

            return new GSTournamentsOpRequest(GetRequestContext()).Send("getLiveTournamentRewards", OnTournamentsOpSuccess, jsonObj.ToString());
        }

        public IPromise<BackendResult> TournamentsOpUpdateTournaments()
        {
            return new GSTournamentsOpRequest(GetRequestContext()).Send("updateTournaments", OnTournamentsOpSuccess);
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

            if (response.ScriptData.ContainsKey("league"))
            {
                playerModel.league = response.ScriptData.GetInt("league").Value;
                playerModelUpdatedSignal.Dispatch(playerModel);
            }

            if (response.ScriptData.ContainsKey("trophies"))
            {
                playerModel.trophies = response.ScriptData.GetInt("trophies").Value;
                playerModelUpdatedSignal.Dispatch(playerModel);
            }

            if (response.ScriptData.ContainsKey("inbox"))
            {
                GSData inboxData = response.ScriptData.GetGSData("inbox");
                Dictionary<string, InboxMessage> dict = new Dictionary<string, InboxMessage>();
                FillInbox(dict, inboxData);
                inboxAddMessagesSignal.Dispatch(dict);
                inboxModel.lastFetchedTime = DateTime.UtcNow;
                inboxModel.items = dict;
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
                for (int i = 0; i < joinedTournamentsList.Count; i++)
                {
                    if (joinedTournamentsList[i].id == tournamentId)
                    {
                        joinedTournamentsList[i] = ParseJoinedTournament(tournamentGSData, joinedTournamentsList[i].id);
                    }
                }
            }

            GSData liveTournamentGSData = response.ScriptData.GetGSData(GSBackendKeys.TournamentsOp.LIVE_TOURNAMENT);
            if (liveTournamentGSData != null)
            {
                LiveTournamentData liveTournament = ParseLiveTournament(liveTournamentGSData);
                liveTournament.lastFetchedTime = DateTime.UtcNow;

                tournamentsModel.SetOpenTournament(liveTournament);
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

