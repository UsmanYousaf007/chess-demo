/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-24 10:45:49 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.TLUtils
{
    public static class FormatUtil
    {
        public static string AbbreviateNumber(long value)
        {
            string abbreviation = value.ToString();

            if (value >= 1000000000)
            {
                abbreviation = (value / 1000000000f).ToString("#.##B");
            }
            else if (value >= 1000000)
            {
                abbreviation = (value / 1000000f).ToString("#.##M");
            }
            else if (value >= 10000)
            {
                abbreviation = (value / 1000f).ToString("#.##K");
            }

            return abbreviation;
        }
    }
}
