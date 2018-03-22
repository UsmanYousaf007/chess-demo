/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 19:24:41 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Chess
{
    public class ChessSquare
    {
        public FileRank fileRank;
        public ChessPiece piece;

        public bool Equals(ChessSquare square)
        {
            return (square != null &&
                    this.fileRank.file == square.fileRank.file &&
                    this.fileRank.rank == square.fileRank.rank);
        }
    }
}
