
using PlayFab;
using PlayFab.ClientModels;
using SocialEdge.Modules;

namespace SocialEdge.Requests
{

    public class SocialEdgeSearchPlayerResponse : SocialEdgeRequestResponse<SetTagResult, PlayFabError>
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
    public class SocialEdgeSearchPlayerRequest : SocialEdgeRequest<SocialEdgeSearchPlayerRequest, SocialEdgeSearchPlayerResponse>
    {
        // Request parameters section
        Friends _friend;
        private string friendId { get; set; }
        public SocialEdgeSearchPlayerRequest()
        {
            _friend = new Friends();
            request = this;
        }

        public SocialEdgeSearchPlayerRequest SetFriendUserId(string displayName)
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

        public void Send(string playerId,int skip)
        {
            
            //_friend.Search(playerId,skip,0)
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
