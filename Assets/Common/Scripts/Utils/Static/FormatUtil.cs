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

namespace TurboLabz.Common
{
    public static class FormatUtil
    {
        /*
         * TODO: Test billion case.
         * Use the snippet below to test out the cases for minifying coins
         * 
         * LogUtil.Log(FormatUtil.AbbreviateNumber(100000000366), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(100000000), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(9000000), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(9123456), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(800000), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(812345), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(70000), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(71234), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(6000), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(6123), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(500), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(512), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(40), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(41), "yellow");
         * LogUtil.Log(FormatUtil.AbbreviateNumber(3), "yellow");
         */
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
