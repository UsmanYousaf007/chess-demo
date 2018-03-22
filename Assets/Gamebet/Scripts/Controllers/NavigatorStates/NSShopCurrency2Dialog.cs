/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-21 16:38:53 UTC+05:00

namespace TurboLabz.Gamebet
{
    public class NSShopCurrency2Dialog : NSShop
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.SHOP_CURRENCY_2_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_SHOP_CURRENCY || evt == NavigatorEvent.ESCAPE)
            {
                return new NSShopCurrency();
            }

            return null;
        }
    }
}
