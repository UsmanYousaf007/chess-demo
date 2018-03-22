/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:37:03 UTC+05:00
/// 
/// @description
/// [add_description_here]
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class NSRooms : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.ROOMS);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            LogUtil.Log("I am here " + evt , "yellow");
            if (evt == NavigatorEvent.ESCAPE)
            {
                cmd.loadLobbySignal.Dispatch();
                return null;
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP)
            {
                return new NSShop();
            }
            else if (evt == NavigatorEvent.SHOW_FREE_CURRENCY_1_DLG)
            {
                return new NSFreeCurrency1Dialog();
            }
            else if (evt == NavigatorEvent.SHOW_OUT_OF_CURRENCY_1_DLG)
            {
                return new NSOutOfCurrency1Dialog();
            }
            else if (evt == NavigatorEvent.SHOW_MATCH_MAKING)
            {
                return new NSMatchMaking();
            }

            return null;
        }
    }
}