/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-21 14:54:34 UTC+05:00

namespace TurboLabz.Gamebet
{
    public class NSShopLootBoxes : NSShop
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.SHOP_LOOT_BOXES);
        }
    }
}
