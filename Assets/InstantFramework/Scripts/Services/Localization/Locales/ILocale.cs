/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-05 17:28:42 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface ILocale
    {
        IDictionary<string, string> data { get; }
    }
}
