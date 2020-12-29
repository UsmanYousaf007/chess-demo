/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using GameSparks.Core;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> GetAllStarLeaderboard()
        {
            return new GSGetLeaderboardsRequest(GetRequestContext()).Send("GlobalAllTime", 100, OnGetLeaderboardSuccess);
        }

        private void OnGetLeaderboardSuccess(object r, Action<object> a)
        {
            LeaderboardDataResponse response = (LeaderboardDataResponse)r;
            
            if (response.HasErrors)
            {
                // We can dispatch error signal here
                return;
            }

            foreach (LeaderboardDataResponse._LeaderboardData entry in response.Data)
            {
                AllStarLeaderboardEntry leaderboardEntry = new AllStarLeaderboardEntry();
                leaderboardEntry.rank = (int)entry.Rank;
                leaderboardEntry.score = int.Parse(entry.JSONData["Score"].ToString());
                leaderboardEntry.uploadedPicId = entry.JSONData["DpUrl"].ToString();
                leaderboardEntry.name = entry.JSONData["Name"].ToString();
                leaderboardEntry.countryId = entry.JSONData["Flag"].ToString();
                leaderboardEntry.league = int.Parse(entry.JSONData["League"].ToString());
                leaderboardEntry.facebookUserId = entry.JSONData["FbId"]?.ToString();

                leaderboardModel.allStarLeaderboardEntries.Add(leaderboardEntry);
            }
        }
    }

    #region REQUEST

    public class GSGetLeaderboardsRequest : GSFrameworkRequest
    {
        public GSGetLeaderboardsRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string leaderboardShortCode, int EntryCount, Action<object, Action<object>> onSuccess)
        {
            this.errorCode = BackendResult.GET_LEADERBOARD_FAILED;
            this.onSuccess = onSuccess;

            new LeaderboardDataRequest()
                .SetLeaderboardShortCode(leaderboardShortCode)
                .SetEntryCount(EntryCount)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
