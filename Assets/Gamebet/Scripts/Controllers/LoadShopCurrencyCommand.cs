/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-14 15:59:44 UTC+05:00

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class LoadShopCurrencyCommand : Command
    {// Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateShopCurrencyViewSignal updateShopCurrencyViewSignal { get; set; }

        // Models
        [Inject] public IViewStateModel viewStateModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IShopSettingsModel shopSettingsModel { get; set; }
        [Inject] public IInventoryModel inventoryModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SHOP_CURRENCY);

            viewStateModel.subShopViewId = SubShopViewId.CURRENCY;

            ShopVO shopVo = new ShopVO();

            shopVo.subShopViewId = viewStateModel.subShopViewId;
            shopVo.shopSettings = shopSettingsModel;
            shopVo.inventorySettings = inventoryModel;
            shopVo.playerModel = playerModel;

            updateShopCurrencyViewSignal.Dispatch(shopVo);
        }
    }
}
