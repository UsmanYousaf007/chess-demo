/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-27 03:34:11 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;

namespace TurboLabz.TLUtils
{
    public class PluralFormatProvider : IFormatProvider, ICustomFormatter
    {
        private string PLURAL_FORMAT_IDENTIFIER = "PLRL|";
        private char PLURAL_FORMAT_SEPARATOR = '|';

        public object GetFormat(Type formatType)
        {
            return this;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                return string.Format("{0}", arg);
            }

            string result;
            bool isPluralizeFormat = format.StartsWith(PLURAL_FORMAT_IDENTIFIER);

            if (isPluralizeFormat)
            {
                format = format.Remove(0, PLURAL_FORMAT_IDENTIFIER.Length);

                string[] forms = format.Split(PLURAL_FORMAT_SEPARATOR);
                int form = ((int)arg == 1) ? 0 : 1;
                result = forms[form];
            }
            else
            {
                result = string.Format("{0:" + format + "}", arg);
            }

            return result;
        }
    }
}
