/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:37:03 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess
{
    public class PlayerModel : IPlayerModel
    {
        public string id { get; set; }
        public string activeSkinId { get; set; }

        [PostConstruct]
        public void Load()
        {
            Reset();

            // TODO: load from the saved file here.
        }

        void Reset()
        {
            id = CPUSettings.DEFAULT_PLAYER_ID;
            activeSkinId = "SkinWood";
        }
    }
}
