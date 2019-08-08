/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-23 16:39:02 UTC+05:00
/// 
/// @description
/// [add_description_here]
using UnityEngine;
using System;
using System.Text;

namespace TurboLabz.Chess
{
    [Serializable]
    public class ChessMove
    {
        public FileRank from;
        public FileRank to;
        public ChessPiece piece;
        public string promo;

        private static readonly char[] FILE_MAP = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'};

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Piece: ");
            sb.Append(piece.color);
            sb.Append(" ");
            sb.Append(piece.name);
            sb.Append(" Move: ");
            sb.Append(GetAlgebraicLocation(from));
            sb.Append(" -> ");
            sb.Append(GetAlgebraicLocation(to));
            sb.Append(" Promo: ");
            sb.Append(promo);

            return sb.ToString();
        }

        public string GetAlgebraicLocation(FileRank fileRank)
        {
            return FILE_MAP[fileRank.file].ToString() + (fileRank.rank + 1).ToString();
        }

        public string MoveToString(FileRank fileRank, FileRank to)
        {
            return GetAlgebraicLocation(fileRank) + GetAlgebraicLocation(to);
        }
    }
}
