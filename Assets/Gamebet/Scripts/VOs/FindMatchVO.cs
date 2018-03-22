/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-30 11:25:16 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Gamebet
{
    public struct FindMatchVO
    {
        // Setting findMatchInLastPlayedRoom to true will ignore roomId and
        // FindMatchCommand will find a match in the last room that the game
        // was played in.
        public bool findMatchInLastPlayedRoom;
        public string roomId;
    }
}
