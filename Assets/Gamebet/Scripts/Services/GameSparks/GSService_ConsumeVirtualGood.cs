
using GameSparks.Api.Responses;
using strange.extensions.promise.api;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        public IPromise<BackendResult> ConsumeVirtualGood(int quantity, string shortCode)
        {
            return new GSConsumeVirtualGoodRequest().Send(quantity, shortCode, OnConsumeVirtualGoodSuccess);
        }

        private void OnConsumeVirtualGoodSuccess(ConsumeVirtualGoodResponse response)
        {
            if (response.HasErrors)
            {
                backendErrorSignal.Dispatch(BackendResult.CONSUME_VIRTUAL_GOOD_FAILED);
            }
        }
    }
}
