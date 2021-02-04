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
        public static string AbbreviateNumber(long value, bool showDecimal)
        {
            string abbreviation = value.ToString("N0");

            if (value >= 1000000000)
            {
                abbreviation = (value / 1000000000f).ToString(showDecimal ? "#.#B" : "#B");
            }
            else if (value >= 1000000)
            {
                abbreviation = (value / 1000000f).ToString(showDecimal ? "#.#M" : "#M");
            }
            else if (value >= 10000)
            {
                abbreviation = (value / 1000f).ToString(showDecimal ? "#.#K" : "#K");
            }

            return abbreviation;
        }

        public static string SplitFirstLastNameInitial(string fullName)
        {
            string[] nameSplit = fullName.Split(new char[] {' '}, System.StringSplitOptions.RemoveEmptyEntries);
            string first = "";
            string last = "";
            string firstNameLastInitial = fullName;
            if (nameSplit.Length > 1)
            {
                for (int i = 0; i < nameSplit.Length - 1; i++)
                {
                    first = first + nameSplit[i];
                }
                last = nameSplit[nameSplit.Length - 1];
                string lastInitial = last[0].ToString();
                firstNameLastInitial = first + " " + lastInitial + ".";
            }

            return firstNameLastInitial;
        }
    }
}
