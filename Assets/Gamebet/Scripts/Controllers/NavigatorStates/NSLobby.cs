/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:37:03 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Gamebet
{
    public class NSLobby : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.LOBBY);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_CPU_MENU)
            {
                return new NSCPUMenu();
            }
            else if (evt == NavigatorEvent.SHOW_CPU_PLAY)
            {
                return new NSCPUPlay();
            }
            else if (evt == NavigatorEvent.SHOW_ROOMS)
            {
                return new NSRooms();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP)
            {
                return new NSShop();
            }
            else if (evt == NavigatorEvent.SHOW_INVENTORY)
            {
                return new NSInventory();
            }
            else if (evt == NavigatorEvent.SHOW_PLAYER_PROFILE)
            {
                return new NSPlayerProfile();
            }
            else if (evt == NavigatorEvent.SHOW_FREE_CURRENCY_1_DLG)
            {
                return new NSFreeCurrency1Dialog();
            }

            return null;
        }
    }
}

