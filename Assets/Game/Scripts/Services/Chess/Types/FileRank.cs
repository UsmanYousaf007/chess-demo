/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 17:49:40 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;

namespace TurboLabz.Chess
{
    [Serializable]
    public struct FileRank : IEquatable<FileRank>
    {
        public int file;
        public int rank;

        private static readonly char[] FILE_MAP = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        public static bool operator ==(FileRank fr1, FileRank fr2)
        {
            return fr1.Equals(fr2);
        }

        public static bool operator !=(FileRank fr1, FileRank fr2)
        {
            return !fr1.Equals(fr2);
        }

        public bool Equals(FileRank other)
        {
            // FileRank is a value type, thus we don't have to check for null.
            // if (other == null) return false;

            return ((file == other.file) && (rank == other.rank));
        }

        public override bool Equals(object other)
        {
            // other could be a reference type.
            // The 'is' operator will return false if null.
            if (other is FileRank)
            {
                return Equals((FileRank)other);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return file.GetHashCode() ^ rank.GetHashCode();
        }

        public override string ToString()
        {
            return file + "x" + rank;
        }

        public string GetAlgebraicLocation()
        {
            return FILE_MAP[file].ToString() + (rank + 1).ToString();
        }
    }
}
