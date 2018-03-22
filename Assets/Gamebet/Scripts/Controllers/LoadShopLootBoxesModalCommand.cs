/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-21 13:25:39 UTC+05:00

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class LoadShopLootBoxesModalCommand : Command
    { 
        //command param
        [Inject] public ShopVO shopVo { get; set; }

        // Dispatch signals.
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateShopLootBoxesModalViewSignal updateShopLootBoxesModalViewSignal { get; set; }

        // Models
        [Inject] public IShopSettingsModel shopSettingsModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SHOP_LOOT_BOXES_DLG);

            updateShopLootBoxesModalViewSignal.Dispatch(shopVo);
        }
    }
}
