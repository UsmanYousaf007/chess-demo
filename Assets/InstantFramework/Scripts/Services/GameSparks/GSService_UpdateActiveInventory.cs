

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> UpdateActiveInventory(
            string activeChessSkinsId,
            string activeAvatarsId,
            string activeAvatarsBorderId
        )
        {
            return new GSUpdateActiveInventoryRequest().Send(activeChessSkinsId,
                activeAvatarsId,
                activeAvatarsBorderId);
        }
    }

    #region REQUEST

    public class GSUpdateActiveInventoryRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "UpdateActiveInventory";
        const string ATT_ACTIVE_CHESS_SKINS_ID = "activeChessSkinsId";
        const string ATT_ACTIVE_AVATARS_ID = "activeAvatarsId";
        const string ATT_ACTIVE_AVATARS_BORDER_ID = "activeAvatarsBorderId";

        public IPromise<BackendResult> Send(
            string activeChessSkinsId,
            string activeAvatarsId,
            string activeAvatarsBorderId)
        {
            this.errorCode = BackendResult.UPDATE_ACTIVE_INVENTORY_FAILED;

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_ACTIVE_CHESS_SKINS_ID, activeChessSkinsId)
                .SetEventAttribute(ATT_ACTIVE_AVATARS_ID, activeAvatarsId)
                .SetEventAttribute(ATT_ACTIVE_AVATARS_BORDER_ID, activeAvatarsBorderId)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}

