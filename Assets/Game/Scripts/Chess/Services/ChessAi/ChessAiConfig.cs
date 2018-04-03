/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-11 15:26:26 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Chess
{
    public static class ChessAiConfig
    {
        // Stockfish settings (transmitted as strings)
        public const string SF_CONTEMPT = "100"; // Greater contempt means search for a win takes precdence over search for a draw. So we max this.
        public const string SF_PONDER = "false"; // We don't want the engine to "think" when it is idle. This will chew up battery life.
        public const string SF_MULTIPV = "10"; // This is how we dumb down stock fish by generating 10 alternate moves and picking one from the list.
        public const string SF_SKILL_LEVEL = "0"; // We use multipv to generate moves, therefore skill level is irrelevant and set to 0.
        public const string SF_SLOW_MOVER = "10"; // This makes the engine think less. We use the minimum here because we want to max out performance.
        public const string SF_DEFAULT_SEARCH_DEPTH = "0"; // For now, keep things as fast as possible. Might increase this with difficulty (TODO).
        public const string SF_ONE_MIN_SEARCH_DEPTH = "10"; // Because we want the computer to be aggressive in 1 min games. Might change (TODO).

        // Basic move selection
        public const int MOVES_TO_GENERATE = 10;
        public const int OPENING_MOVES_SELECT_COUNT = 5;
        public const int OPENING_MOVES_COUNT = 5;
        public const int DIFFICULTY_VARIANCE = 1;

        // Panic filters
        public const int ONE_MIN_PANIC_CHANCE = 50;
        public const int THIRTY_SECOND_PANIC_CHANCE = 60;
        public const int TEN_SECOND_PANIC_CHANCE = 70;
        public const int PANIC_MOVE_INDEX = MOVES_TO_GENERATE - 1;

        // Dice rolls
        public const int PIECE_EXCHANCE_CHANCE = 50;

        // Special situations
        public const int END_GAME_PIECE_COUNT = 4;
        public const int ROOK_RESTRICTION_MOVE_COUNT = 10;

        // Fixed settings .. DO NOT MODIFY consts below this
        public const int KING_VALUE = 1000;
        public const int QUEEN_VALUE = 100;
        public const int ROOK_VALUE = 75;
        public const int BISHOP_VALUE = 50;
        public const int KNIGHT_VALUE = 50;
        public const int PAWN_VALUE = 25;
    }
}
