﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using PlayFab;
using PlayFab.ClientModels;

namespace SocialEdge.Requests
{
    public class SocialEdgeBlockFriendResponse : SocialEdgeRequestResponse<SetFriendTagsResult, PlayFabError>
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(SetFriendTagsResult resultSuccess)
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
    public class SocialEdgeBlockFriendRequest : SocialEdgeRequest<SocialEdgeBlockFriendRequest, SocialEdgeBlockFriendResponse>
    {
        // Request parameters section
        public string friendId { get; set; }

        public SocialEdgeBlockFriendRequest()
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
            var request = new SetFriendTagsRequest
            {
                FriendPlayFabId = friendId
            };

            PlayFabClientAPI.SetFriendTags(request, OnSuccess, OnFailure);
        }

        private void OnSuccess(SetFriendTagsResult result)
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
