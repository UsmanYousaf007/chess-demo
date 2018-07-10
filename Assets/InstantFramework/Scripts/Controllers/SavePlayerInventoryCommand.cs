

using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class SavePlayerInventoryCommand : Command
    {
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
<<<<<<< HEAD
            TLUtils.LogUtil.Log("SavePlayerInventoryCommand()  " + playerModel.activeSkinId);
=======
>>>>>>> origin/devel
            backendService.UpdateActiveInventory(playerModel.activeSkinId).Then(OnComplete);
        }

        private void OnComplete(BackendResult result)
        {
<<<<<<< HEAD
            TLUtils.LogUtil.Log("SavePlayerInventoryCommand::OnComplete()", "cyan");

            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);

                TLUtils.LogUtil.Log("SavePlayerInventoryCommand::OnComplete() success!", "cyan");
=======
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
>>>>>>> origin/devel
            }

            Release();
        }
    }
}
