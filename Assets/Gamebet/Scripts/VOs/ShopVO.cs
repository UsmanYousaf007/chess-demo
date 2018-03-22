/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-14 12:10:59 UTC+05:00
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class ShopVO
    {
        public IPlayerModel playerModel;

        public IShopSettingsModel shopSettings;
        public IInventoryModel inventorySettings;
        public IForgeSettingsModel forgeSettingsModel;

        public SubShopViewId subShopViewId;
        public SubInventoryViewId subInventoryViewId;
        public string activeShopItemId;
        public string activeInventoryItemId;
        public string activeForgeCardItemId;

        //public string activeChessSkinsId;
        //public string activeAvatarsId;
        //public string activeAvatarsBorderId;
    }
}