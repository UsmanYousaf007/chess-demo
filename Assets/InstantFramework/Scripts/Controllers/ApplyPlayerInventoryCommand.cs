

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

        private static string activeAvatarsIdCache = "unassigned";
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
                    (activeAvatarsIdCache != playerModel.activeAvatarId) ||
                    (activeChessSkinsIdCache != playerModel.activeSkinId)
                );

            // Apply avatar
            // TODO: Set just the ID at this layer and not sprite
            /*
            string activeAvatarId = inventoryModel.activeAvatarsId ?? "unassigned";
            Sprite activeAvatarThumb = AvatarThumbsContainer.container.GetThumb(activeAvatarId);
            if (activeAvatarThumb != null)
            {
                if (activeAvatarId == "AvatarFacebook")
                {
                    playerModel.profilePicture = playerModel.profilePictureFB;
                }
                else
                {
                    playerModel.profilePicture = activeAvatarThumb;
                }
            }
            */

            // Update inventory on the back end
            if (isBackendUpdateRequired)
            {
                backendService.UpdateActiveInventory(playerModel.activeSkinId, 
                    playerModel.activeAvatarId,
                    null).Then(OnProcessApplyPlayerInventory);
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
                activeAvatarsIdCache = playerModel.activeAvatarId;
                activeChessSkinsIdCache = playerModel.activeSkinId;
            }

            Release();
        }
    }
}
