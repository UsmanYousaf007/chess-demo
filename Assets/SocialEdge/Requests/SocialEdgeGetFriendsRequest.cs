/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using PlayFab;
using PlayFab.ClientModels;

namespace SocialEdge.Requests
{
    public class SocialEdgeGetFriendsResponse : SocialEdgeRequestResponse<GetFriendsListResult, PlayFabError>
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(GetFriendsListResult resultSuccess)
        {

            isSuccess = true;
            token = resultSuccess.Request.AuthenticationContext.EntityToken;
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
    public class SocialEdgeGetFriendsRequest : SocialEdgeRequest<SocialEdgeGetFriendsRequest, SocialEdgeGetFriendsResponse>
    {
        // Request parameters section
        public string friendId { get; set; }

        public SocialEdgeGetFriendsRequest()
        {
            // Mandatory call to base class
            Base(this);
        }


        /// <summary>
        /// Execute the request
        /// </summary>
        ///
        public override void Send()
        {
            var request = new GetFriendsListRequest();

            PlayFabClientAPI.GetFriendsList(request, OnSuccess, OnFailure);
        }

        private void OnSuccess(GetFriendsListResult result)
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
}
