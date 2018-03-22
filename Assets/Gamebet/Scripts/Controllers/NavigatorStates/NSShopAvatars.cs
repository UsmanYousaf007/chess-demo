/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-21 14:55:06 UTC+05:00

namespace TurboLabz.Gamebet
{
    public class NSShopAvatars : NSShop
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.SHOP_AVATARS);
        }
    }
}
