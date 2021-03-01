/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-25 15:05:46 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Chess
{
    public enum AiMoveDelay
    {
        NONE,               // Used for hints
        CPU,                // For play vs computer
        ONLINE_1M,          // For online 1 m matches
        ONLINE_3M,          // For online 3 m matches
        ONLINE_5M,          // For online 5 m matches
        ONLINE_10M,         // For online 10 m matches
        ONLINE_30M,         // For online 30 m matches
    }
}
