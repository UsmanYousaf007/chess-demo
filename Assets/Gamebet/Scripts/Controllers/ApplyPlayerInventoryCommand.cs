

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class ApplyPlayerInventoryCommand : Command
    {
        // Models
        [Inject] public IInventoryModel inventoryModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        public override void Execute()
        {
            Retain();
            ProcessApplyPlayerInventory();
        }

        public void ProcessApplyPlayerInventory()
        {
            // Apply avatar
            string activeAvatarId = inventoryModel.activeAvatarsId ?? "unassigned";
            AvatarThumb activeAvatarThumb = AvatarThumbsContainer.container.GetThumb(activeAvatarId);
            if (activeAvatarThumb != null)
            {
                if (activeAvatarId == "AvatarFacebook")
                {
                    playerModel.profilePicture = playerModel.profilePictureFB;
                }
                else
                {
                    playerModel.profilePicture = activeAvatarThumb.thumbnail;
                }
            }

            // Apply avatar border
            string activeAvatarBorderId = inventoryModel.activeAvatarsBorderId ?? "unassigned";
            AvatarBorderThumb activeAvatarBorderThumb = AvatarBorderThumbsContainer.container.GetThumb(activeAvatarBorderId);
            if (activeAvatarBorderThumb != null)
            {
                playerModel.profilePictureBorder = activeAvatarBorderThumb.thumbnail;
            }

            // Apply chess skin
            string activeChessSkinsId = inventoryModel.activeChessSkinsId ?? "unassigned";
            TurboLabz.MPChess.SkinContainer activeChessSkin = TurboLabz.MPChess.SkinContainer.container;
            if (activeChessSkin != null)
            {
                activeChessSkin.Unload();
            }

            // TODO need a skinning interface! a reference to chess namespace is no good.
            TurboLabz.MPChess.SkinContainer.LoadSkin(activeChessSkinsId);

            // Update inventory on the back end
            backendService.UpdateActiveInventory(inventoryModel.activeChessSkinsId, 
                inventoryModel.activeAvatarsId,
                inventoryModel.activeAvatarsBorderId).Then(OnProcessApplyPlayerInventory);
        }

        private void OnProcessApplyPlayerInventory(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}
