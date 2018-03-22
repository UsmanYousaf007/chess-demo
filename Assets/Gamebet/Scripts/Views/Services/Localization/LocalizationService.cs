/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-05 16:35:40 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class LocalizationService : ILocalizationService
    {
        // The data dictionary contains all the key-value pairs for the selected
        // locale.
        private IDictionary<string, string> data = new Dictionary<string, string>();

        private IDictionary<string, ILocale> locales = new Dictionary<string, ILocale>() {
            { "en-US", new Locale_en_US() },
            { "x", new Locale_x() }
        };

        private string _locale;

        public string locale
        {
            get
            {
                return _locale;
            }

            set
            {
                Assertions.Assert(locales.ContainsKey(value), "The locale " + value + " does not exist in the locales DB!");

                _locale = value;
                data = locales[_locale].data;
            }
        }

        public LocalizationService()
        {
            // Change locale to set the default language.
            locale = "en-US";
        }

        public string Get(string key, params object[] args)
        {
            Assertions.Assert(data.ContainsKey(key), "The key " + key + " does not exist in the " + locale + " locale file!");

            return string.Format(new PluralFormatProvider(), data[key], args);
        }

        public string GetRoomTitle(string key, params object[] args)
        {
            string roomTitleKey = (key == RoomTitleId.NONE) ? LocalizationKey.ROOM_TITLE_NONE : key;
            return Get(roomTitleKey, args);
        }
    }
}
