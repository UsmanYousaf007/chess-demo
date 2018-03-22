/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-21 13:24:55 UTC+05:00

using strange.extensions.command.impl;
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class LoadShopCurrency1ModalCommand : Command
    { 
        //command param
        [Inject] public ShopVO shopVo { get; set; }

        // Dispatch signals.
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateShopCurrency1ModalViewSignal updateShopCurrency1ModalViewSignal { get; set; }

        // Models
        [Inject] public IShopSettingsModel shopSettingsModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SHOP_CURRENCY_1_DLG);

            updateShopCurrency1ModalViewSignal.Dispatch(shopVo);
        }
    }
}
