

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> UpdateActiveInventory(
            string activeChessSkinsId
        )
        {
            return new GSUpdateActiveInventoryRequest(GetRequestContext()).Send(activeChessSkinsId);
        }
    }

    #region REQUEST

    public class GSUpdateActiveInventoryRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "UpdateActiveInventory";
        const string ATT_ACTIVE_CHESS_SKINS_ID = "activeChessSkinsId";

        public GSUpdateActiveInventoryRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(
            string activeChessSkinsId)
        {
            this.errorCode = BackendResult.UPDATE_ACTIVE_INVENTORY_FAILED;

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_ACTIVE_CHESS_SKINS_ID, activeChessSkinsId)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}

