
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace TurboLabz.Gamebet
{
    public class BuildForgeCardsCommand : Command
    {
        // Models
        [Inject] public IInventoryModel inventoryModel { get; set; }
        [Inject] public IForgeSettingsModel forgeSettingsModel { get; set; }
        [Inject] public IShopSettingsModel shopSettingsModel { get; set; }

        // VO
        [Inject] public string forgeCardKey { get; set; }
        [Inject] public Signal buildFogeCardsResultSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        public override void Execute()
        {
            Retain();
            ProcessBuildForgeCard (forgeCardKey);
        }

        public void ProcessBuildForgeCard(string cardId)
        {  
            // Request forge operation on the backend
            backendService.GrantForgedItem(cardId).Then(OnProcessForge);
        }

        private void OnProcessForge(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            if (buildFogeCardsResultSignal != null)
            {
                buildFogeCardsResultSignal.Dispatch();
            }

            Release();
        }
    }
}
