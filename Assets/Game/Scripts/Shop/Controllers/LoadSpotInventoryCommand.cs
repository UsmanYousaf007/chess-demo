using strange.extensions.command.impl;
using TurboLabz.Multiplayer;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class LoadSpotInventoryCommand : Command
    {
        // Command Params
        [Inject] public LoadSpotInventoryParams spotInventoryParams { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateSpotInventoryViewSignal updateSpotInventoryViewSignal { get; set; }

        // Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_INVENTORY);

            var vo = new SpotInventoryVO();
            vo.storeItem = metaDataModel.store.items[spotInventoryParams.itemShortCode];
            vo.currentRewardedPoints = playerModel.GetInventoryItemCount($"{spotInventoryParams.itemShortCode}Points");
            vo.requiredRewardedPoints = metaDataModel.settingsModel.GetInventorySpecialItemsRewardedVideoCost(spotInventoryParams.itemShortCode);
            vo.gemsCount = playerModel.gems;
            vo.icon = StoreIconsContainer.Load().GetSprite(spotInventoryParams.itemShortCode);
            vo.itemToUnlockShortCode = spotInventoryParams.itemToUnclockShortCode;
            vo.enableVideoButton = true;

            if (!string.IsNullOrEmpty(spotInventoryParams.challengeId) && chessboardModel.isValidChallenge(spotInventoryParams.challengeId))
            {
                vo.enableVideoButton = chessboardModel.chessboards[spotInventoryParams.challengeId].backendPlayerTimer.TotalMinutes >= 3;
            }

            updateSpotInventoryViewSignal.Dispatch(vo);
        }
    }
}
