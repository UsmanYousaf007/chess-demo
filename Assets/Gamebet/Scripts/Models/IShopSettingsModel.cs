/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-02 02:45:23 UTC+05:00

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public interface IShopSettingsModel
    {
        IOrderedDictionary<string, SkinShopItem> skinShopItems { get; set; }
        IOrderedDictionary<string, CurrencyShopItem> currencyShopItems { get; set; }
        IOrderedDictionary<string, AvatarShopItem> avatarShopItems { get; set; }
        IOrderedDictionary<string, ChatpackShopItem> chatpackShopItems { get; set; }
        IOrderedDictionary<string, AvatarBorderShopItem> avatarBorderShopItems { get; set; }
        IOrderedDictionary<string, ForgeCardShopItem> forgeCardShopItems { get; set; }
        IOrderedDictionary<string, LootBoxShopItem> lootBoxShopItems { get; set; }

        IOrderedDictionary<string, ShopItem> allShopItems { get; set; }
    }
}

