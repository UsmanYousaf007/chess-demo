/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-21 16:35:44 UTC+05:00

namespace TurboLabz.Gamebet
{
    public class NSShopLootBoxesDialog : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.SHOP_LOOT_BOXES_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_SHOP_LOOT_BOXES || evt == NavigatorEvent.ESCAPE)
            {
                return new NSShopLootBoxes();
            }

            return null;
        }
    }
}
