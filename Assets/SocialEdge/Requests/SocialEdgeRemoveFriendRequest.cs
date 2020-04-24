/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using PlayFab;
using PlayFab.ClientModels;

namespace SocialEdge.Requests
{
    public class SocialEdgeRemoveFriendResponse : SocialEdgeRequestResponse<RemoveFriendResult, PlayFabError>
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(RemoveFriendResult resultSuccess)
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
    public class SocialEdgeRemoveFriendRequest : SocialEdgeRequest<SocialEdgeRemoveFriendRequest, SocialEdgeRemoveFriendResponse>
    {
        // Request parameters section
        private string friendId { get; set; }

        public SocialEdgeRemoveFriendRequest()
        {
            // Mandatory call to base class
            Base(this);
        }

        public SocialEdgeRemoveFriendRequest SetFriendUserId(string displayName)
        {
            friendId = displayName;
            return this;
        }

        /// <summary>
        /// Execute the request
        /// </summary>
        ///
        public override void Send()
        {
            var request = new RemoveFriendRequest
            {
                FriendPlayFabId = friendId
            };

            PlayFabClientAPI.RemoveFriend(request, OnSuccess, OnFailure);
        }

        private void OnSuccess(RemoveFriendResult result)
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
