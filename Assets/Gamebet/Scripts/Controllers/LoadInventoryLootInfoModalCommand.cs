/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-08 16:46:10 UTC+05:00
using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class LoadInventoryLootInfoModalCommand : Command
    {
        //command param
        [Inject] public ShopVO shopVo { get; set; }

        // Dispatch signals.
        [Inject] public LoadModalViewSignal loadModalViewSignal { get; set; }
        [Inject] public UpdateInventoryLootInfoModalViewSignal updateInventoryLootInfoModalViewSignal { get; set; }

        // Models
        [Inject] public IShopSettingsModel shopSettingsModel { get; set; }

        public override void Execute()
        {
            loadModalViewSignal.Dispatch(ModalViewId.InventoryLootInfo);

            updateInventoryLootInfoModalViewSignal.Dispatch(shopVo);
        }
    }
}