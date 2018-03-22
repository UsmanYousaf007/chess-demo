/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-21 16:37:21 UTC+05:00

namespace TurboLabz.Gamebet
{
    public class NSShopChessSkinsDialog : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.SHOP_CHESS_SKINS_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_SHOP_CHESS_SKINS || evt == NavigatorEvent.ESCAPE)
            {
                return new NSShopChessSkins();
            }
            else if (evt == NavigatorEvent.SHOW_INVENTORY)
            {
                return new NSInventory();
            }

            return null;
        }
    }
}
