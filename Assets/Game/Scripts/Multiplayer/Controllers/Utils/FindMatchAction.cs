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
        public struct ActionData
        {
            public string action;
            public string opponentId;
            public string matchGroup;
            public bool isRanked;
            public string avatarId;
            public string avatarBgColor;
        }

        public const string ACTION_RANDOM = "Random";
        public const string ACTION_RANDOM_LONG = "RandomLong";
        static ActionData actionData = new ActionData();
        public static bool isMatchRequestedWithFriend;

        static public void Reset()
        {
            actionData.action = "unassigned";
            actionData.opponentId = "unassinged";
            actionData.matchGroup = "unassinged";
            actionData.isRanked = true;
        }

        static public void Random(FindMatchSignal signal)
        {
            Reset();
            isMatchRequestedWithFriend = false;
            actionData.action = "Random";
            signal.Dispatch(JsonUtility.ToJson(actionData));
        }

        static public void Challenge(FindMatchSignal signal, bool isRanked, string opponentId)
        {
            Reset();
            isMatchRequestedWithFriend = true;
            actionData.action = "Challenge";
            actionData.isRanked = isRanked;
            actionData.opponentId = opponentId;
            signal.Dispatch(JsonUtility.ToJson(actionData));
        }

        static public void RandomLong(FindMatchSignal signal)
        {
            Reset();
            isMatchRequestedWithFriend = false;
            actionData.action = "RandomLong";
            signal.Dispatch(JsonUtility.ToJson(actionData));
        }

        static public void Accept(FindMatchSignal signal, string opponentId, string matchGroup, string avatarId, string avatarBgColor)
        {
            Reset();
            isMatchRequestedWithFriend = false;
            actionData.action = "Accept";
            actionData.matchGroup = matchGroup;
            actionData.opponentId = opponentId;
            actionData.avatarId = avatarId;
            actionData.avatarBgColor = avatarBgColor;
            signal.Dispatch(JsonUtility.ToJson(actionData));
        }
    }
}
