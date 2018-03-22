/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-20 12:21:36 UTC+05:00
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class NSInventory : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.INVENTORY);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            LogUtil.Log("I am " + evt , "yellow");

            if (evt == NavigatorEvent.ESCAPE || evt == NavigatorEvent.BACK_FROM_INVENTORY)
            {
                NavigatorViewId cameFromId = CameFrom(
                    NavigatorViewId.LOBBY,
                    NavigatorViewId.ROOMS,
                    NavigatorViewId.SHOP,
                    NavigatorViewId.SHOP_CHESS_SKINS_DLG,
                    NavigatorViewId.SHOP_AVATARS_DLG,
                    NavigatorViewId.SHOP_AVATARS_BORDER_DLG
                );

                LogUtil.Log("Came from ID  " + cameFromId , "yellow");

                if (cameFromId == NavigatorViewId.LOBBY)
                {
                    cmd.loadLobbySignal.Dispatch();
                    return null;
                }
                else if (cameFromId == NavigatorViewId.ROOMS)
                {
                    cmd.loadRoomsSignal.Dispatch();
                    return null;
                }
                else if (cameFromId == NavigatorViewId.SHOP)
                {
                    cmd.loadShopSignal.Dispatch();
                    return null;
                }
                else if (cameFromId == NavigatorViewId.SHOP_CHESS_SKINS_DLG)
                {
                    cmd.loadShopSignal.Dispatch();
                    return null;
                }
                else if (cameFromId == NavigatorViewId.SHOP_AVATARS_DLG)
                {
                    cmd.loadShopSignal.Dispatch();
                    return null;
                }
                else if (cameFromId == NavigatorViewId.SHOP_AVATARS_BORDER_DLG)
                {
                    cmd.loadShopSignal.Dispatch();
                    return null;
                }
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_ROOMS)
            {
                return new NSRooms();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP)
            {
                return new NSShop();
            }

            return null;
        }
    }
}