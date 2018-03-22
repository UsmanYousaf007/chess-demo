/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-12 13:14:32 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Gamebet
{
    public class GSFormat
    {
        public static int GetBool(bool val) 
        {
            return (val ? 1 : 0);
        }

        public static string GetOptionalString(string val)
        {
            return ((val == null) ? "" : val);
        }
    }
}
