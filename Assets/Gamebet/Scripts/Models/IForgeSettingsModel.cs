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

namespace TurboLabz.Gamebet
{
    public interface IForgeSettingsModel
    {
        // Keys for this dictionary are card ids.
        IOrderedDictionary<string, ForgeItem> forgeItems { get; set; }
    }
}