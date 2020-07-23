using strange.extensions.promise.api;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> SetLastWatchedVideo(string videoId)
        {
            return new GSSetLastWatchedVideoRequest(GetRequestContext()).Send(videoId);
        }
    }

    #region REQUEST

    public class GSSetLastWatchedVideoRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "SetLastWatchedVideo";
        const string ATT_VIDEO_ID = "videoId";

        public GSSetLastWatchedVideoRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string videoId)
        {
            this.errorCode = BackendResult.UPDATE_ACTIVE_INVENTORY_FAILED;

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_VIDEO_ID, videoId)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}

