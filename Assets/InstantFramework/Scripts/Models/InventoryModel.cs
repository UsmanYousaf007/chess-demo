/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class InventoryModel : IInventoryModel
    {
        public void Reset()
        {
            activeChessSkinId = null;
            activeAvatarsId = null;
            activeAvatarsBorderId = null;
            items = null;
        }

        public string activeChessSkinId { get; set; }
        public string activeAvatarsId { get; set; }
        public string activeAvatarsBorderId { get; set; }

        public IOrderedDictionary<string, int> items { get; set; }
    }

}
