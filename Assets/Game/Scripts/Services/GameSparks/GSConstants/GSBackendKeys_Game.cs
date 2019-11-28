/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-22 20:03:21 UTC+05:00
/// 
/// @description
/// These are the server backend keys. These keys remain the same on the server
/// and the client side.

using System.Collections.Generic;

using TurboLabz.Chess;

namespace TurboLabz.InstantFramework
{
    public static partial class GSBackendKeys
    {
        public const string GAME_DURATION = "gameDuration";
        public const string TIMER = "timer";
        public const string COLOR = "playerColor";
        public const string LAST_MOVE = "lastMove";
        public const string FROM_SQUARE = "fromSquare";
        public const string TO_SQUARE = "toSquare";
        public const string PROMOTION = "promotion";
        public const string MOVE_FLAG = "moveFlag";
        public const string GAME_END_HAS_MOVE = "gameEndHasMove";
        public const string GAME_END_REASON = "gameEndReason";
        public const string IS_FIFTY_MOVE_RULE_ACTIVE = "isFiftyMoveRuleActive";
        public const string IS_THREEFOLD_REPEAT_RULE_ACTIVE = "isThreefoldRepeatRuleActive";
        public const string COMPLETED_CHALLENGE_ID = "COMPLETE";
        public const string BOT_ID = "shardFlag";
        public const string CURRENT_TURN_PLAYER_ID = "currentTurnPlayerId";
        public const string NEXT_TURN_PLAYER_ID = "nextTurnPlayerId";
        public const string GAME_DATA = "gameData";
        public const string FEN = "fen";
        public const string MOVE_LIST = "moveList";
        public const string IS_CAPTURE = "isCapture";
        public const string TEST_CHESS_CONFIG = "testChessConfig";
        public const string AI_DIFFICULTY = "aiDifficulty";
        public const string AI_RESIGN_BEHAVIOUR = "aiResignBehaviour";
        public const string AI_SPEED = "aiSpeed";
        public const string TIME_STAMP = "timestamp";
        public const string POWER_UP_USED_COUNT = "powerupUsedCount";

        // Message ExtCodes
        public const string CHALLENGE_ACCEPT_MESSAGE = "ChallengeAcceptMessage";

        // Challenge response keys
        public const string CHALLENGE_INSTANCE_ID = "challengeInstanceId";

        public const long ROOM_1_DURATION = 1 * 60 * 1000; // 1 minute (milliseconds)
        public const long ROOM_2_DURATION = 3 * 60 * 1000; // 3 minute (milliseconds)
        public const long ROOM_3_DURATION = 5 * 60 * 1000; // 5 minute (milliseconds)
        public const long ROOM_4_DURATION = 10 * 60 * 1000; // 10 minute (milliseconds)

        public static readonly Dictionary<string, ChessMoveFlag> MOVE_FLAG_MAP = new Dictionary<string, ChessMoveFlag>() {
            { "standard", ChessMoveFlag.STANDARD },
            { "castleQueenSide", ChessMoveFlag.CASTLE_QUEEN_SIDE },
            { "castleKingSide", ChessMoveFlag.CASTLE_KING_SIDE },
            { "pawnPromotionQueen", ChessMoveFlag.PAWN_PROMOTION_QUEEN },
            { "pawnPromotionRook", ChessMoveFlag.PAWN_PROMOTION_ROOK },
            { "pawnPromotionBishop", ChessMoveFlag.PAWN_PROMOTION_BISHOP },
            { "pawnPromotionKnight", ChessMoveFlag.PAWN_PROMOTION_KNIGHT },
            { "enPassant", ChessMoveFlag.EN_PASSANT }
        };

        public static readonly Dictionary<string, GameEndReason> GAME_END_REASON_MAP = new Dictionary<string, GameEndReason>() {
            { "checkmate", GameEndReason.CHECKMATE },
            { "stalemate", GameEndReason.STALEMATE },
            { "timerExpired", GameEndReason.TIMER_EXPIRED },
            { "resignation", GameEndReason.RESIGNATION },
            { "drawByInsufficientMaterial", GameEndReason.DRAW_BY_INSUFFICIENT_MATERIAL },
            { "drawByFiftyMovesWithMove", GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITH_MOVE },
            { "drawByFiftyMovesWithoutMove", GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITHOUT_MOVE },
            { "drawByThreefoldRepetitionWithMove", GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITH_MOVE },
            { "drawByThreefoldRepetitionWithoutMove", GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITHOUT_MOVE },
            { "playerDisconnected", GameEndReason.PLAYER_DISCONNECTED },
            { "declined", GameEndReason.DECLINED },
            { "abandon", GameEndReason.ABANDON}
        };

        public static readonly Dictionary<string, ChessColor> PLAYER_COLOR_MAP = new Dictionary<string, ChessColor>() {
            { "white", ChessColor.WHITE },
            { "black", ChessColor.BLACK }
        };
    }

    // TODO: Organize these like the event data keys, consider class renaming
    // TODO: Look at the ErrorKey being sent from the server and wire it up to our error screen
}
