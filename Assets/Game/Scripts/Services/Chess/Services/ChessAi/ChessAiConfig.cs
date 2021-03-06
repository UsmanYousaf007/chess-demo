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
using UnityEngine;

namespace TurboLabz.Chess
{
    public static class ChessAiConfig
    {
        // Stockfish settings (transmitted as strings)
        public const string SF_CONTEMPT = "100"; // Greater contempt means search for a win takes precdence over search for a draw. So we max this.
        public const string SF_PONDER = "false"; // Let the engine think while player is thinking, so it moves faster.
        public const string SF_MULTIPV = "10"; // This is how we dumb down stock fish by generating 10 alternate moves and picking one from the list.
        public const string SF_SKILL_LEVEL = "19"; // We use multipv and custom code to generate moves, therefore DO NOT change this value
        //public const string SF_SLOW_MOVER = "10"; // This makes the engine think less. No documentation on this.
        //public const string SF_HASH = "128"; // This is a value in MB and it may improve performance as you increase it (testing pending)
        public const string SF_ANALYSIS_MULTIPV = "20";

        // Duration difficulty thresholds
        public const double DURATION_FAST_MINS = 1;
        public const double DURATION_MEDIUM_MINS = 10;
        public const float FAST_GIMP = 0.5f;
        public const float MEDIUM_GIMP = 0.8f;

        // Search depth is key to making stockfish smarter
        public const int SF_MIN_SEARCH_DEPTH = 0; 
        public const int SF_MAX_SEARCH_DEPTH = 12;
        public const int SF_ANALYSIS_SEARCH_DEPTH = 12;

        // Basic move selection
        public const int OPENING_MOVES_SELECT_COUNT = 3;
        public const int OPENING_MOVES_COUNT = 3;
        public const float OPENING_MOVE_PROPER_CHANCE = 0.5f;
        public const int DIFFICULTY_VARIANCE = 1;

        // Panic filters
        public const int ONE_MIN_PANIC_CHANCE = 50;
        public const int THIRTY_SECOND_PANIC_CHANCE = 60;
        public const int TEN_SECOND_PANIC_CHANCE = 70;

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
