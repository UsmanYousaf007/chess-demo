/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-24 15:26:55 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using TurboLabz.Common;
using System;
using UnityEngine;

namespace TurboLabz.Gamebet
{
    public struct RoomsVO
    {
        public long currency1;
        public long currency2;
        public int numberOfLockedRoomsToDisplay;
        public string cardIdOfHighestAffordableRoom;

        // The keys of the dictionary are the IDs of the rooms.
        // RoomInfo holds all the room specific information.
        public IOrderedDictionary<string, RoomInfo> rooms;

        // The keys of the dictionary are the IDs of the rooms.
        // RoomRecord and RoomRecordVO hold all the player specific info
        // for the room.
        public IDictionary<string, RoomRecordVO> roomRecords;

    }

    public class RoomInfo
    {
        public RoomInfo(RoomSetting setting)
        {
            this.setting = setting;
        }

        public bool isLocked;
        public bool isMystery;
        public TimeSpan timeRemaining;
        public TimeSpan timeBuffer;
        public Coroutine roomCountdownCR;
        public Coroutine nextRoomCountdownCR;

        public readonly RoomSetting setting;
    }
}
