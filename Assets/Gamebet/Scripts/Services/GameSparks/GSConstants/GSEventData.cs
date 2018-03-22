/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-14 16:44:28 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Gamebet 
{
    public static partial class GSEventData
    {
        public const ushort ATT_VAL_TRUE = 1;
        public const ushort ATT_VAL_FALSE = 0;

        public static class Ping
        {
            public const string EVT_KEY = "Ping";
            public const string ATT_KEY_CLIENT_SEND_TIMESTAMP = "clientSendTimestamp";
            public const string ATT_KEY_SERVER_RECEIPT_TIMESTAMP = "serverReceiptTimestamp";
        }

        public static class FindMatch
        {
            public const string EVT_KEY = "FindMatch";
            public const string ATT_KEY_MATCH_GROUP = "matchGroup";
            // ATT_VAL_MATCH_GROUP will be provided by the roomId
            public const string ATT_KEY_IS_BOT_MATCH = "shardFlag";
        }

        public static class GetGameStartTime
        {
            public const string EVT_KEY = "GetGameStartTime";
            public const string ATT_KEY_CHALLENGE_ID = "challengeId";
        }

        public static class SetPlayerSocialName
        {
            public const string EVT_KEY = "SetPlayerSocialName";
            public const string ATT_KEY_NAME = "name";
        }

        public static class GetInitData
        {
            public const string EVT_KEY = "GetInitData";
        }

        public static class ClaimAdReward
        {
            public const string EVT_KEY = "ClaimAdReward";
        }

        public static class GetAppInfo
        {
            public const string EVT_KEY = "GetAppInfo";
        }

        public static class GrantForgedItem
        {
            public const string EVT_KEY = "GrantForgedItem";
            public const string ATT_KEY_FORGE_CARD_KEY = "forgeCardKey";
        }

        public static class SellForgeCards
        {
            public const string EVT_KEY = "SellForgeCards";
            public const string ATT_KEY_FORGE_CARD_KEY = "forgeCardKey";
            public const string ATT_KEY_QUANTITY = "quantity";
        }

        public static class ClaimLoot
        {
            public const string EVT_KEY = "ClaimLoot";
            public const string ATT_KEY_CLAIM_KEY = "key";
        }

        public static class UpdateActiveInventory
        {
            public const string EVT_KEY = "UpdateActiveInventory";
            public const string ATT_KEY_ACTIVE_CHESS_SKINS_ID = "activeChessSkinsId";
            public const string ATT_KEY_ACTIVE_AVATARS_ID = "activeAvatarsId";
            public const string ATT_KEY_ACTIVE_AVATARS_BORDER_ID = "activeAvatarsBorderId";
        }

        // TODO: Remove this test code
        public const string TEST = "TestEvent";
    }
}
