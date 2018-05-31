

using GameSparks.Api.Responses;
using strange.extensions.promise.api;

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
                activeAvatarsBorderId,
                OnUpdateActiveInventorySuccess);
        }

        private void OnUpdateActiveInventorySuccess(LogEventResponse response)
        {
            if (response.HasErrors)
            {
                backendErrorSignal.Dispatch(BackendResult.UPDATE_ACTIVE_INVENTORY_FAILED);
            }
        }
    }
}

