/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    [Serializable]
    public static class FindMatchAction
    {

        public enum ActionCode
        {
            Random,
            Random10,
            RandomLong,
            Challenge,
            Accept,
            Challenge10,
            Random1,
            Challenge1
        }

        public enum NotificationStatus
        {
            InGame,
            OutGame,
            None
        }

        public struct ActionData
        {
            public string action;
            public string opponentId;
            public string matchGroup;
            public bool isRanked;
            public string avatarId;
            public string avatarBgColor;
            public NotificationStatus notificationStatus;
            public string acceptActionCode;
        }

        public const string ACTION_RANDOM = "Random";
        public static ActionData actionData = new ActionData();
        public static bool isMatchRequestedWithFriend;
        public static bool isRandomLongMatch;

        static public void Reset()
        {
            actionData.action = "unassigned";
            actionData.opponentId = "unassinged";
            actionData.matchGroup = "unassinged";
            actionData.isRanked = true;
        }

        static public void Random(FindMatchSignal signal, string actionCode)
        {
            Reset();
            isMatchRequestedWithFriend = false;
            isRandomLongMatch = false;
            actionData.action = actionCode;
            actionData.notificationStatus = NotificationStatus.None;
            signal.Dispatch(JsonUtility.ToJson(actionData));
        }

        static public void Challenge(FindMatchSignal signal, bool isRanked, string opponentId, string actionCode)
        {
            Reset();
            isMatchRequestedWithFriend = true;
            isRandomLongMatch = false;
            //actionData.action = "Challenge";
            actionData.action = actionCode;
            actionData.isRanked = isRanked;
            actionData.opponentId = opponentId;
            actionData.notificationStatus = NotificationStatus.None;
            signal.Dispatch(JsonUtility.ToJson(actionData));
        }

        static public void RandomLong(FindMatchSignal signal)
        {
            Reset();
            isMatchRequestedWithFriend = false;
            isRandomLongMatch = true;
            actionData.action = ActionCode.RandomLong.ToString();
            actionData.notificationStatus = NotificationStatus.None;
            signal.Dispatch(JsonUtility.ToJson(actionData));
        }

        static public void Accept(FindMatchSignal signal, string opponentId, string matchGroup, string avatarId, string avatarBgColor, string actionCode, NotificationStatus notificationStatus)
        {
            Reset();
            isMatchRequestedWithFriend = false;
            isRandomLongMatch = false;
            //actionData.action = "Accept";
            actionData.action = ActionCode.Accept.ToString();
            actionData.matchGroup = matchGroup;
            actionData.opponentId = opponentId;
            actionData.avatarId = avatarId;
            actionData.avatarBgColor = avatarBgColor;
            actionData.notificationStatus = notificationStatus;
            actionData.acceptActionCode = actionCode;
            signal.Dispatch(JsonUtility.ToJson(actionData));
        }
    }
}
