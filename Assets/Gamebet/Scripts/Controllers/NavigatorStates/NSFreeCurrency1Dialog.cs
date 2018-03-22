/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-20 14:38:26 UTC+05:00
using TurboLabz.Common;
using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    public class NSFreeCurrency1Dialog : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.FREE_CURRENCY_1_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE || evt == NavigatorEvent.CLOSE_DLG)
            {

                NavigatorViewId cameFromId = CameFrom(
                    NavigatorViewId.LOBBY,
                    NavigatorViewId.ROOMS,
                    NavigatorViewId.SHOP
                );

                LogUtil.Log("The " + evt , "yellow");
                if (cameFromId == NavigatorViewId.LOBBY)
                {
                    LogUtil.Log("Lobby " + evt , "yellow");
                    return new NSLobby();
                }
                else if (cameFromId == NavigatorViewId.ROOMS)
                {
                    LogUtil.Log("Room " + evt , "yellow");
                    return new NSRooms();
                }
                else if (cameFromId == NavigatorViewId.SHOP)
                {
                    return new NSShop();
                }
            }

            return null;
        }
    }
}
