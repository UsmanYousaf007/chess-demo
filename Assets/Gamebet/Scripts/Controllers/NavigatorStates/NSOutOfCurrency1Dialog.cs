/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-20 17:45:47 UTC+05:00

namespace TurboLabz.Gamebet
{
    public class NSOutOfCurrency1Dialog : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.OUT_OF_CURRENCY_1_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE || evt == NavigatorEvent.CLOSE_DLG)
            {
                if (IsPreviousView(NavigatorViewId.ROOMS))
                {
                    return new NSRooms();
                }
            }
            else if (evt == NavigatorEvent.SHOW_SHOP)
            {
                return new NSShop();
            }

            return null;
        }
    }
}
