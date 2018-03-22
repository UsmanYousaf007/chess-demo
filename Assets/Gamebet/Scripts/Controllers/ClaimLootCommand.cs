

using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace TurboLabz.Gamebet
{
    public class ClaimLootCommand : Command
    {
        // Models
        [Inject] public IInventoryModel inventoryModel { get; set; }
        [Inject] public IForgeSettingsModel forgeSettingsModel { get; set; }
        [Inject] public IPlayerModel playersModel { get; set; }

        // VO
        [Inject] public string lootBoxKey { get; set; }
        [Inject] public Signal claimLootResultSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        public override void Execute()
        {
            Retain();
            ProcessClaimLoot(lootBoxKey);
        }

        public void ProcessClaimLoot(string lootBoxKey)
        {
            // Request claim loot
            backendService.ClaimLoot(lootBoxKey).Then(OnProcessClaimLoot);;
        }

        private void OnProcessClaimLoot(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            if (claimLootResultSignal != null)
            {
                claimLootResultSignal.Dispatch();
            }

            Release();
        }
    }
}
