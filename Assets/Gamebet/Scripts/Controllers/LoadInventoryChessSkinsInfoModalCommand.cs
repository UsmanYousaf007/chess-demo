/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-27 15:06:30 UTC+05:00

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class LoadInventoryChessSkinsInfoModalCommand : Command
    {
        //command param
        [Inject] public ShopVO shopVo { get; set; }

        // Dispatch signals.
        [Inject] public LoadModalViewSignal loadModalViewSignal { get; set; }
        [Inject] public UpdateInventoryChessSkinsInfoModalViewSignal updateInventoryChessSkinsInfoModalViewSignal { get; set; }

        // Models
        [Inject] public IShopSettingsModel shopSettingsModel { get; set; }

        public override void Execute()
        {
            loadModalViewSignal.Dispatch(ModalViewId.InventoryChessSkins);

            updateInventoryChessSkinsInfoModalViewSignal.Dispatch(shopVo);
        }
    }
}
