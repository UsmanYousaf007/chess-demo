

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

            // TODO: This is hardcoded to only handle skins. Make this generic for new item types
            backendService.UpdateActiveInventory(playerModel.activeSkinId, !string.IsNullOrEmpty(requestJson) ? requestJson : null).Then(OnComplete);
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
