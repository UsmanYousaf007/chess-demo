/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
//using PlayFab;
//using PlayFab.ClientModels;

namespace SocialEdge.Requests
{
 /*   
    public class SocialEdgeUpdatePlayerDataResponse : SocialEdgeRequestResponse<UpdateUserDataResult, PlayFabError>
    {

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(UpdateUserDataResult resultSuccess)
        {
            isSuccess = true;
        }

        /// <summary>
        /// Build results on request failure
        /// </summary>
        public override void BuildFailure(PlayFabError resultFailure)
        {
            isSuccess = false;
        }
    }

    /// <summary>
    /// Backend login to server request
    /// </summary>
    public class SocialEdgeUpdatePlayerDataRequest : SocialEdgeRequest<SocialEdgeUpdatePlayerDataRequest, SocialEdgeUpdatePlayerDataResponse>
    {
        // Request parameters section

        private Dictionary<string, string> userData;
        public SocialEdgeUpdatePlayerDataRequest()
        {
            request = this;
        }

        public SocialEdgeUpdatePlayerDataRequest SetUserData(PlayerModel player)
        {
            
            //userData["tag"] = player.privateProfile.tag;
            //userData["eloCompletedPlacementGames"] = player.privateProfile.eloCompletedPlacementGames.ToString();
            //userData["eloScore"] = player.privateProfile.eloScore.ToString();
            //userData["gamesWon"] = player.privateProfile.gamesWon.ToString();
            //userData["gamesLost"] = player.privateProfile.gamesLost.ToString();
            //userData["gamesDrawn"] = player.privateProfile.gamesDrawn.ToString();
            //userData["countryFlag"] = player.privateProfile.countryFlag;
            

            return this;
        }
        /// <summary>
        /// Execute the request
        /// </summary>
        ///
        public override void Send()
        {
            var request = new UpdateUserDataRequest
            {
                Data = userData,

            };

            PlayFabClientAPI.UpdateUserData(request, OnSuccess, OnFailure);
        }

        private void OnSuccess(UpdateUserDataResult result)
        {
            response.BuildSuccess(result);
            actionSuccess?.Invoke(response);
        }

        private void OnFailure(PlayFabError error)
        {
            response.BuildFailure(error);
            actionFailure?.Invoke(response);
        }
    }
    */
}