/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-30 14:56:23 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class RoomSettingsModel : IRoomSettingsModel
    {
        // The keys of the dictionary are the IDs of the rooms.
        public IOrderedDictionary<string, RoomSetting> settings { get; set; }
    }

    public class RoomSetting
    {
        public string id;
        public string groupId;
        public int unlockAtLevel;
        public long gameDuration; // Milliseconds, TODO(mubeeniqbal): Convert from long to TimeSpan
        public long wager;
        public long prize;
        public long drawPrize;
        public int victoryXp;
        public int winsForTrophy;
        public int trophiesForRoomTitle1;
        public int trophiesForRoomTitle2;
        public int trophiesForRoomTitle3;
        public string roomDescription;
        public long roomStartTime;  // Rotating room open start time in ms
        public int roomDuration;    // Rotating room duration in ms. Buffer is included in duration.
        public int roomBuffer;      // Rotating room Buffer in ms.
    }
}
