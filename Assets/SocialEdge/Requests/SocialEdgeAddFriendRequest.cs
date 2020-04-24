/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using PlayFab;
using PlayFab.ClientModels;

namespace SocialEdge.Requests
{
    public class SocialEdgeAddFriendResponse : SocialEdgeRequestResponse<SetTagResult, PlayFabError>
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(SetTagResult resultSuccess)
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
    public class SocialEdgeAddFriendRequest : SocialEdgeRequest<SocialEdgeAddFriendRequest, SocialEdgeAddFriendResponse>
    {
        // Request parameters section

        private string friendId { get; set; }
        public SocialEdgeAddFriendRequest()
        {
            request = this;
        }

        public SocialEdgeAddFriendRequest SetFriendUseId(string displayName)
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
            var request = new AddFriendRequest
            {
                FriendPlayFabId = friendId

            };

            PlayFabClientAPI.AddFriend(request, OnSuccess, OnFailure);
        }

        private void OnSuccess(SetTagResult result)
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
