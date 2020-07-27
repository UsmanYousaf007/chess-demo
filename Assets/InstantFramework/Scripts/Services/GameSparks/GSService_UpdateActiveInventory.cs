

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> UpdateActiveInventory(
            string activeChessSkinsId,
            string json = null)
        {
            return new GSUpdateActiveInventoryRequest(GetRequestContext()).Send(activeChessSkinsId, json);
        }
    }

    #region REQUEST

    public class GSUpdateActiveInventoryRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "UpdateActiveInventory";
        const string ATT_ACTIVE_CHESS_SKINS_ID = "activeChessSkinsId";
        const string ATT_JSON = "json";

        public GSUpdateActiveInventoryRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(
            string activeChessSkinsId,
            string json = null)
        {
            this.errorCode = BackendResult.UPDATE_ACTIVE_INVENTORY_FAILED;

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_ACTIVE_CHESS_SKINS_ID, activeChessSkinsId)
                .SetEventAttribute(ATT_JSON, json)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}

