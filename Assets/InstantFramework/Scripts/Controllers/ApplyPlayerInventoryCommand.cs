

using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ApplyPlayerInventoryCommand : Command
    {
        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        private static string activeChessSkinsIdCache = "unassigned";

        public override void Execute()
        {
            Retain();
            ProcessApplyPlayerInventory();
        }

        public void ProcessApplyPlayerInventory()
        {
            bool isBackendUpdateRequired = 
                (
                    (activeChessSkinsIdCache != playerModel.activeSkinId)
                );
                    
            // Update inventory on the back end
            if (isBackendUpdateRequired)
            {
                backendService.UpdateActiveInventory(playerModel.activeSkinId).Then(OnProcessApplyPlayerInventory);
            }
        }

        private void OnProcessApplyPlayerInventory(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }
            else
            {
                activeChessSkinsIdCache = playerModel.activeSkinId;
            }

            Release();
        }
    }
}
