/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-04-14 13:07:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;

namespace TurboLabz.Common
{
    public static class CollectionsUtil
    {
        private static Random random = new Random();

        public static void Shuffle<T>(T[] collection)
        {
            int length = collection.Length;

            // Knuth shuffle algorithm.
            for (int i = 0; i < length; ++i)
            {
                int index = random.Next(i, length);
                T tmp = collection[i];
                collection[i] = collection[index];
                collection[index] = tmp;
            }
        }
    }
}
