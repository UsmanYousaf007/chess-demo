/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-23 11:02:31 UTC+05:00

using strange.extensions.command.impl;
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class LoadInventoryAvatarsCommand : Command
    {
        // Dispatch signals
        [Inject] public LoadViewSignal loadViewSignal { get; set; }
        [Inject] public UpdateInventoryAvatarsViewSignal updateInventoryAvatarsViewSignal { get; set; }

        // Models
        [Inject] public IViewStateModel viewStateModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IShopSettingsModel shopSettingsModel { get; set; }
        [Inject] public IInventoryModel inventoryModel { get; set; }
        [Inject] public IForgeSettingsModel forgeSettingsModel { get; set; }

        public override void Execute()
        {
            //loadViewSignal.Dispatch(ViewId.INVENTORY);

            viewStateModel.subInventoryViewId = SubInventoryViewId.AVATARS;

            ShopVO shopVo = new ShopVO();

            shopVo.subInventoryViewId = viewStateModel.subInventoryViewId;
            shopVo.shopSettings = shopSettingsModel;
            shopVo.inventorySettings = inventoryModel;
            shopVo.playerModel = playerModel;
            shopVo.forgeSettingsModel = forgeSettingsModel;

            updateInventoryAvatarsViewSignal.Dispatch(shopVo);
        }
    }
}
