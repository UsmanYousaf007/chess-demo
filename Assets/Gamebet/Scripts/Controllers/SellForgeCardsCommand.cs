
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace TurboLabz.Gamebet
{
    public class SellForgeCardsCommand : Command
    {
        // Models
        [Inject] public IInventoryModel inventoryModel { get; set; }
        [Inject] public IForgeSettingsModel forgeSettingsModel { get; set; }
        [Inject] public IPlayerModel playersModel { get; set; }

        // VO
        [Inject] public ForgeCardVO forgeCardVO { get; set; }
        [Inject] public Signal sellForgeCardsResultSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        public override void Execute()
        {
            Retain();
            ProcessSellForgeCards(forgeCardVO.cardId, forgeCardVO.numSell);
        }

        public void ProcessSellForgeCards(string cardId, int numSell)
        {
            // Request sell forge cards on backend
            backendService.SellForgeCards(cardId, numSell).Then(OnProcessSellForgeCard);;
        }

        private void OnProcessSellForgeCard(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            if (sellForgeCardsResultSignal != null)
            {
                sellForgeCardsResultSignal.Dispatch();
            }

            Release();
        }
    }
}
