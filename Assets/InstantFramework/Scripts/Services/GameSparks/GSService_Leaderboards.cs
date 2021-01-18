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
            /*
            "Score": score,
            "Name": publicProfile.name,
            "CountryId": publicProfile.countryId,
            "League": publicProfile.league,
            "FbId": publicProfile.externalIds['FB'],
            "UploadedPicId": publicProfile.uploadedPicId,
            "AvatarId": avatarId,
            "AvatarBgColorId": avatarBgColorId
            */

            LeaderboardDataResponse response = (LeaderboardDataResponse)r;
            
            if (response.HasErrors)
            {
                // We can dispatch error signal here
                return;
            }

            foreach (LeaderboardDataResponse._LeaderboardData entry in response.Data)
            {
                AllStarLeaderboardEntry leaderboardEntry = new AllStarLeaderboardEntry();

                leaderboardEntry.score = int.Parse(entry.JSONData["Score"].ToString());
                leaderboardEntry.playerId = entry.JSONData["userId"].ToString();
                leaderboardEntry.rank = (int)entry.Rank;

                // Public profile
                leaderboardEntry.publicProfile = new PublicProfile();
                leaderboardEntry.publicProfile.playerId = entry.JSONData["userId"].ToString();
                leaderboardEntry.publicProfile.name = entry.JSONData["Name"].ToString();
                leaderboardEntry.publicProfile.countryId = entry.JSONData["CountryId"].ToString();
                leaderboardEntry.publicProfile.league = int.Parse(entry.JSONData["League"].ToString());
                leaderboardEntry.publicProfile.avatarBgColorId = entry.JSONData["AvatarBgColorId"]?.ToString();
                leaderboardEntry.publicProfile.uploadedPicId = entry.JSONData["UploadedPicId"].ToString();
                leaderboardEntry.publicProfile.avatarId = entry.JSONData["AvatarId"]?.ToString();

                string fbId = entry.JSONData["FbId"]?.ToString();
                if (string.IsNullOrEmpty(fbId))
                {
                    fbId = null;
                }
                else if (fbId == "null")
                {
                    fbId = null;
                }
                leaderboardEntry.publicProfile.facebookUserId = fbId;

                leaderboardEntry.name = leaderboardEntry.publicProfile.name;

                var leagueAssets = tournamentsModel.GetLeagueSprites(leaderboardEntry.publicProfile.league.ToString());
                leaderboardEntry.publicProfile.leagueBorder = leagueAssets != null ? leagueAssets.ringSprite : null;

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
