/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-24 15:58:53 UTC+05:00
///
/// @description
/// [add_description_here]
using System.Collections.Generic;

using UnityEngine;

using System.Collections.Generic;

using strange.extensions.command.impl;

using TurboLabz.Common;
using System;

namespace TurboLabz.Gamebet
{
    public class LoadRoomsCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadViewSignal loadViewSignal { get; set; }
        [Inject] public UpdateRoomsViewSignal updateRoomsViewSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IRoomSettingsModel roomSettingsModel { get; set; }

        public override void Execute()
        {
            // Display the menu
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_ROOMS);
            //loadViewSignal.Dispatch(ViewId.ROOMS);

            RoomsVO vo = new RoomsVO();
            vo.rooms = new OrderedDictionary<string, RoomInfo>();

            vo.currency1 = playerModel.currency1;
            vo.currency2 = playerModel.currency2;
            vo.numberOfLockedRoomsToDisplay = RoomsConstants.numberOfLockedRooms;

            bool firstStandardRooms = true;
            bool firstRotatingRoom = true;
            int testCounter = 0;

            long cycleTime = 0; 
            long rotatingRoomsStartTime = 0;

            foreach (KeyValuePair<string, RoomSetting> roomSetting in roomSettingsModel.settings)
            {
                RoomInfo roomInfo = new RoomInfo(roomSetting.Value);

                if (roomInfo.setting.groupId == RoomsConstants.ROTATING_ROOMS_GROUP_ID)
                {
                    cycleTime += roomInfo.setting.roomDuration;

                    if (firstRotatingRoom)
                    {
                        rotatingRoomsStartTime = roomInfo.setting.roomStartTime;
                        firstRotatingRoom = false;
                    }
                }
            }

            //long cycleNumber = (TimeUtil.unixTimestampMilliseconds - rotatingRoomsStartTime)/cycleTime;
            long currentTimeValue = (TimeUtil.unixTimestampMilliseconds - rotatingRoomsStartTime)%cycleTime;

            long rotatingRoomStartTime = 0;
            long rotatingRoomDurationTime = 0;
            long rotatingRoomEndTime = 0;

            foreach(KeyValuePair<string, RoomSetting> roomSetting in roomSettingsModel.settings)
            {
                RoomInfo roomInfo = new RoomInfo(roomSetting.Value);

                if (roomInfo.setting.groupId == RoomsConstants.ROTATING_ROOMS_GROUP_ID)
                {
                    ++testCounter;

                    rotatingRoomDurationTime += roomInfo.setting.roomDuration;

                    rotatingRoomStartTime = rotatingRoomDurationTime - roomInfo.setting.roomDuration;

                    rotatingRoomEndTime = rotatingRoomDurationTime;

                    bool isInTime = (currentTimeValue > rotatingRoomStartTime) && (currentTimeValue < rotatingRoomEndTime);

                    long msTimeRemainingIncludingBuffer = rotatingRoomEndTime - currentTimeValue;
                    long msTimeRemaining = (rotatingRoomEndTime - currentTimeValue) - roomInfo.setting.roomBuffer;
                    TimeSpan timeRemaining = TimeSpan.FromMilliseconds(msTimeRemaining);

                    if(isInTime)
                    {
                        if (roomInfo.setting.unlockAtLevel > playerModel.level)
                        {
                            roomInfo.isLocked = true;
                        }

                        long msTimeBufferRemaining = 0;

                        if (msTimeRemainingIncludingBuffer < roomInfo.setting.roomBuffer)
                        {
                            msTimeBufferRemaining = msTimeRemainingIncludingBuffer;
                        }
                        else
                        {
                            msTimeBufferRemaining = roomInfo.setting.roomBuffer;
                        }

                        TimeSpan timeBuffer = TimeSpan.FromMilliseconds(msTimeBufferRemaining);

                        roomInfo.timeBuffer = timeBuffer;
                        roomInfo.timeRemaining = timeRemaining;
                        vo.rooms.Add(roomInfo.setting.id, roomInfo);
                    }
                }

                else if (roomInfo.setting.groupId == RoomsConstants.ALL_IN_ONE_ROOMS_GROUP_ID)
                {
                    if (roomInfo.setting.unlockAtLevel > playerModel.level)
                    {
                        roomInfo.isLocked = true;
                    }

                    vo.rooms.Add(roomInfo.setting.id, roomInfo);
                }

                else if(roomInfo.setting.groupId == RoomsConstants.STANDARD_ROOMS_GROUP_ID)
                {
                    if (roomInfo.setting.unlockAtLevel <= playerModel.level)
                    {
                        roomInfo.isLocked = false;

                        if (firstStandardRooms)
                        {
                            vo.cardIdOfHighestAffordableRoom = roomInfo.setting.id;
                            firstStandardRooms = false;
                        }

                        if (playerModel.currency1 >= roomInfo.setting.wager)
                        {
                            vo.cardIdOfHighestAffordableRoom = roomInfo.setting.id;
                        }

                        vo.rooms.Add(roomInfo.setting.id, roomInfo);
                    }

                    else if(vo.numberOfLockedRoomsToDisplay > 0)
                    {
                        roomInfo.isLocked = true;

                        vo.rooms.Add(roomInfo.setting.id, roomInfo);

                        --vo.numberOfLockedRoomsToDisplay;
                    }

                    else if(vo.numberOfLockedRoomsToDisplay == 0)
                    {
                        roomInfo.isMystery = true;
                        vo.rooms.Add(roomInfo.setting.id, roomInfo);
                        break;
                    }
                }
            }

            IDictionary<string, RoomRecordVO> roomRecords = new Dictionary<string, RoomRecordVO>();

            foreach (RoomRecord record in playerModel.roomRecords.Values)
            {
                string roomId = record.id;

                RoomSetting roomInfo = roomSettingsModel.settings[roomId];

                RoomRecordVO recordVO;
                recordVO.id = roomId;
                recordVO.gameDuration = roomInfo.gameDuration;
                recordVO.gamesWon = record.gamesWon;
                recordVO.gamesLost = record.gamesLost;
                recordVO.gamesDrawn = record.gamesDrawn;
                recordVO.trophiesWon = record.trophiesWon;
                recordVO.roomTitleId = record.roomTitleId;

                roomRecords.Add(roomId, recordVO);
            }

            vo.roomRecords = roomRecords;

            updateRoomsViewSignal.Dispatch(vo);
        }
    }
}
