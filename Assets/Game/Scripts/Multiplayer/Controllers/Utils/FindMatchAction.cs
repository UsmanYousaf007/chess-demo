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
            Random30,
            RandomLong,
            Challenge,
            Accept,
            Challenge10,
            Challenge30,
            Random1,
            Challenge1,
            Random3,
            Challenge3
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
            public string tournamentId;
            public long betValue;
            public bool powerMode;
            public string testGroup;
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

        static public void Random(FindMatchSignal signal, MatchInfoVO matchInfoVO, string tournamentId = "")
        {
            Reset();
            isMatchRequestedWithFriend = false;
            isRandomLongMatch = false;
            actionData.action = matchInfoVO.actionCode;
            actionData.betValue = matchInfoVO.betValue;
            actionData.powerMode = matchInfoVO.powerMode;
            actionData.notificationStatus = NotificationStatus.None;
            actionData.tournamentId = string.IsNullOrEmpty(tournamentId) ? "" : tournamentId;
            actionData.testGroup = Settings.ABTest.COINS_TEST_GROUP;
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
            actionData.betValue = 0;
            actionData.powerMode = false;
            actionData.notificationStatus = NotificationStatus.None;
            actionData.testGroup = Settings.ABTest.COINS_TEST_GROUP;
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
            actionData.betValue = 0;
            actionData.powerMode = false;
            actionData.notificationStatus = notificationStatus;
            actionData.acceptActionCode = actionCode;
            actionData.testGroup = Settings.ABTest.COINS_TEST_GROUP;
            signal.Dispatch(JsonUtility.ToJson(actionData));
        }

        static public bool IsRandomMatch()
        {
            if (actionData.action == ActionCode.Random1.ToString() ||
                actionData.action == ActionCode.Random3.ToString() ||
                actionData.action == ActionCode.Random.ToString() ||
                actionData.action == ActionCode.Random10.ToString() ||
                actionData.action == ActionCode.Random30.ToString() ||
                actionData.action == ActionCode.RandomLong.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
         }
    }
}
