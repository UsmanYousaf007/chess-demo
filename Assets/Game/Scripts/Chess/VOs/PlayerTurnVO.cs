/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-18 18:21:50 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Chess
{
    public struct PlayerTurnVO
    {
        public ChessSquare fromSquare;
        public ChessSquare toSquare;
        public string promo;
        public bool claimFiftyMoveDraw;
        public bool claimThreefoldRepeatDraw;
        public bool rejectThreefoldRepeatDraw;
    }

    // For Multiplayer games:
    // The reason we use the rejectThreefoldRepeatDraw flag is to load the game
    // by move history on the server.
    //
    // Threefold repeat draws are calculated on the client since its too much
    // processing to load the game from game history on the server each time.
    // This is how the flow works:
    //
    // 1. Client: Player makes moves and detects that he can claim a threefold
    //    repeat.
    // 2. Client: Player rejects the threefold repeat and sends that flag to the
    //    server.
    // 3. Server: Server sees that the player has rejected a repeat, what that
    //    means is that a repeat draw claim is available for the opponent. So it
    //    loads the game from history so chess.js can detect the threefold
    //    repeat and offer it to the opponent.  
}
