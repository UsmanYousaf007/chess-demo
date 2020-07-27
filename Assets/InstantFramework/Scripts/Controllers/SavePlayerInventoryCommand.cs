

using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class SavePlayerInventoryCommand : Command
    {
        // Params
        [Inject] public string requestJson { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        public override void Execute()
        {
            Retain();

            // If we have a Json then it's generic, otherwise we send the skin id for backwards compatibility
            if (!string.IsNullOrEmpty(requestJson))
            {
                // The string "unassigned" is here because this is how it is handled in GS cloud code.
                backendService.UpdateActiveInventory("unassigned", requestJson).Then(OnComplete);
            }
            else
            {
                backendService.UpdateActiveInventory(playerModel.activeSkinId).Then(OnComplete);
            }
        }

        private void OnComplete(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}
