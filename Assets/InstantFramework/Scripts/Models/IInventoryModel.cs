/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface IInventoryModel
    {
        void Reset();
        string activeChessSkinId { get; set; }
        string activeAvatarsId { get; set; }
        string activeAvatarsBorderId { get; set; }

        IOrderedDictionary<string, int> items { get; set; }
    }
}
