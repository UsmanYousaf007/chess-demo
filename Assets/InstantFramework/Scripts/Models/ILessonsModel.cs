/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface ILessonsModel
    {
        Dictionary<string, Dictionary<string, List<string>>> lessonsMapping { get; set; }
    }
}
