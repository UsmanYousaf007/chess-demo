/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-22 12:28:11 UTC+05:00

using strange.extensions.command.impl;
using TurboLabz.Common;
using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    public class LoadInventoryCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateInventoryViewSignal updateInventoryViewSignal { get; set; }

        // Models
        [Inject] public IViewStateModel viewStateModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IShopSettingsModel shopSettingsModel { get; set; }
        [Inject] public IInventoryModel inventoryModel { get; set; }
        [Inject] public IForgeSettingsModel forgeSettingsModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_INVENTORY);

            ShopVO shopVo = new ShopVO();

            if (viewStateModel.subInventoryViewId == SubInventoryViewId.LOOT)
            {
                viewStateModel.subInventoryViewId = SubInventoryViewId.AVATARS;
            }

            shopVo.subInventoryViewId = viewStateModel.subInventoryViewId;
            shopVo.shopSettings = shopSettingsModel;
            shopVo.inventorySettings = inventoryModel;
            shopVo.playerModel = playerModel;
            shopVo.forgeSettingsModel = forgeSettingsModel;

            updateInventoryViewSignal.Dispatch(shopVo);
        }
    }
}