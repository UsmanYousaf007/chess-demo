/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-02 02:45:32 UTC+05:00

using TurboLabz.Common;
using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    public class ShopSettingsModel : IShopSettingsModel
    {
        // The keys of the dictionary are the IDs of the shop items.
        public IOrderedDictionary<string, SkinShopItem> skinShopItems { get; set; }
        public IOrderedDictionary<string, CurrencyShopItem> currencyShopItems { get; set; }
        public IOrderedDictionary<string, AvatarShopItem> avatarShopItems { get; set; }
        public IOrderedDictionary<string, ChatpackShopItem> chatpackShopItems { get; set; }
        public IOrderedDictionary<string, AvatarBorderShopItem> avatarBorderShopItems { get; set; }
        public IOrderedDictionary<string, ForgeCardShopItem> forgeCardShopItems { get; set; }
        public IOrderedDictionary<string, LootBoxShopItem> lootBoxShopItems { get; set; }

        public IOrderedDictionary<string, ShopItem> allShopItems { get; set; }
    }

    public class ShopItem
    {
        public string id;                   // Short Code
        public string kind;                 // Classification
        public string type;                 // VGOOD or CURRENCY
        public string tier;                 // common, rare, epic, legendary
        public int maxQuantity;             // Max quantity allowed to purchase
        public string displayName;          
        public string description;          
        public int currency1Cost;           // Cost in currency1 or payout 
                                            // in case of CURRENCY type
        public int currency2Cost;           // Cost in currency2
    }

    public class SkinShopItem : ShopItem
    {
        public int unlockAtLevel;
    }

    public class CurrencyShopItem : ShopItem
    {
        public string promotionId;
        public float bonusXpPercentage;
        public int hintsCount;
        public float lossRecoveryPercentage;
        public int bonusAmount;
    }

    public class AvatarShopItem : ShopItem
    {
        public int unlockAtLevel;
    }

    public class ChatpackShopItem : ShopItem
    {
        public int unlockAtLevel;           
    }

    public class AvatarBorderShopItem : ShopItem
    {
        public int unlockAtLevel;
    }
        
    public class ForgeCardShopItem : ShopItem
    {
        public int unlockAtLevel;
    }

    public class LootBoxShopItem : ShopItem
    {
        public string weightTier;
        public int unlockAtLevel;
        public IList<string> itemDescriptions;
    }
}
