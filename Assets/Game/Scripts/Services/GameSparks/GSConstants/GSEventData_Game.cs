/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-09 12:20:48 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Gamebet 
{
    public static partial class GSEventData
    {
        public static class TakeTurn
        {
            public const string EVT_KEY = "TakeTurn";
            public const string ATT_KEY_FROM = "from";
            public const string ATT_KEY_TO = "to";
            public const string ATT_KEY_PROMOTION = "promotion";
            public const string ATT_KEY_CLAIM_FIFTY_MOVE_DRAW = "claimFiftyMoveDraw";
            public const string ATT_KEY_CLAIM_THREEFOLD_REPEAT_DRAW = "claimThreefoldRepeatDraw";
            public const string ATT_KEY_REJECT_THREEFOLD_REPEAT_DRAW = "rejectThreefoldRepeatDraw";
        }

        public static class AiTurn
        {
            public const string EVT_KEY = "SetShardFlag";
            public const string ATT_KEY_CHALLENGE_ID = "challengeId";
            public const string ATT_KEY_FROM = "shard1";
            public const string ATT_KEY_TO = "shard2";
            public const string ATT_KEY_PROMOTION = "shard3";
        }

        public static class AiResign
        {
            public const string EVT_KEY = "SetShardFlag2";
            public const string ATT_KEY_CHALLENGE_ID = "challengeId";
        }

        public static class ClaimFiftyMoveDraw
        {
            public const string EVT_KEY = "ClaimFiftyMoveDraw";
        }

        public static class ClaimThreefoldRepeatDraw
        {
            public const string EVT_KEY = "ClaimThreefoldRepeatDraw";
        }

        public static class Resign
        {
            public const string EVT_KEY = "Resign";
            public const string ATT_KEY_CHALLENGE_ID = "challengeId";
        }
    }
}

