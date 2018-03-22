/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Noor Khawaja <noor.khawaja@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-30 14:56:23 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class ForgeItem
    {
        public string forgeItemKey;     // Id of item to forge
        public int requiredQuantity;    // Required number of cards to validate forge
        public int sellCoins;           // Number of coins earned on sell
    }

    public class ForgeSettingsModel : IForgeSettingsModel
    {
        // Keys for this dictionary are card ids.
        public IOrderedDictionary<string, ForgeItem> forgeItems { get; set; }
    }
}