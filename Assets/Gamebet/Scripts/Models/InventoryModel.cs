/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Noor Khawaja <noor.khawaja@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-02 02:45:23 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Common;
using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    public class InventoryModel : IInventoryModel
    {
        public string activeChessSkinsId { get; set; }
        public string activeAvatarsId { get; set; }
        public string activeAvatarsBorderId { get; set; }

        public IOrderedDictionary<string, int> allShopItems { get; set; }
        public IList<LootBox> lootBoxItems { get; set; }
    }
        
    public class LootShopItem
    {
        public string shopItemKey;
        public int quantity;
    }

    public class LootBox
    {
        public string key;
        public string lootBoxKey;
        public int coins;
        public List<LootShopItem> shopItems;
    }

}
