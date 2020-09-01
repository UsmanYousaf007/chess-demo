/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.TLUtils;
using System.Linq;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class LeaguesModel : ILeaguesModel
    {
        public Dictionary<string, League> leagues { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            leagues = new Dictionary<string, League>();
        }
    }
}
