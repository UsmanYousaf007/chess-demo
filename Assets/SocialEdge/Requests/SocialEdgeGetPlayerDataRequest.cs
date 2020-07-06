
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ProfilesModels;
using SocialEdge.Modules;

namespace SocialEdge.Requests
{

    public class SocialEdgeGetPlayerDataResponse : SocialEdgeRequestResponse<GetUserDataResult, PlayFabError>
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(GetUserDataResult resultSuccess)
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
    public class SocialEdgeGetPlayerDataRequest : SocialEdgeRequest<SocialEdgeGetPlayerDataRequest, SocialEdgeGetPlayerDataResponse>
    {
        // Request parameters section
        
        public SocialEdgeGetPlayerDataRequest()
        {
            request = this;
        }


        /// <summary>
        /// Execute the request
        /// </summary>
        ///


        public override void Send()
        {
            var request = new GetUserDataRequest
            {
               

            };
       //     PlayFabProfilesAPI.GetProfile(new
       //PlayFab.ProfilesModels.GetEntityProfileRequest(),OnSuccessObj,OnFailure);
            PlayFabClientAPI.GetUserData(request, OnSuccess, OnFailure);
        }

        private void OnSuccessObj(GetEntityProfileResponse abc)
        { }

        private void OnSuccess(GetUserDataResult result)
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
