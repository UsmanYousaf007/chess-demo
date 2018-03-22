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
    public class NSShop : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.SHOP);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            //LogUtil.Log("I am " + evt , "yellow");

            if (evt == NavigatorEvent.ESCAPE || evt == NavigatorEvent.BACK_FROM_SHOP)
            {
                NavigatorViewId cameFromId = CameFrom(
                    NavigatorViewId.LOBBY,
                    NavigatorViewId.ROOMS
                );

                //LogUtil.Log("Came from ID  " + cameFromId , "yellow");

                if (cameFromId == NavigatorViewId.LOBBY)
                {
                    cmd.loadLobbySignal.Dispatch();
                    return null;
                }
                else if (cameFromId == NavigatorViewId.ROOMS || cameFromId == NavigatorViewId.OUT_OF_CURRENCY_1_DLG)
                {
                    cmd.loadRoomsSignal.Dispatch();
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
            else if (evt == NavigatorEvent.SHOW_FREE_CURRENCY_1_DLG)
            {
                return new NSFreeCurrency1Dialog();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP_LOOT_BOXES)
            {
                return new NSShopLootBoxes();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP_AVATARS)
            {
                return new NSShopAvatars();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP_CHESS_SKINS)
            {
                return new NSShopChessSkins();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP_CHAT)
            {
                return new NSShopChat();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP_CURRENCY)
            {
                return new NSShopCurrency();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP_LOOT_BOXES_DLG)
            {
                return new NSShopLootBoxesDialog();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP_AVATARS_DLG)
            {
                return new NSShopAvatarsDialog();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP_AVATARS_BORDER_DLG)
            {
                return new NSShopAvatarsBorderDialog();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP_CHESS_SKINS_DLG)
            {
                return new NSShopChessSkinsDialog();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP_CURRENCY_1_DLG)
            {
                return new NSShopCurrency1Dialog();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP_CURRENCY_2_DLG)
            {
                return new NSShopCurrency2Dialog();
            }

            return null;
        }
    }
}