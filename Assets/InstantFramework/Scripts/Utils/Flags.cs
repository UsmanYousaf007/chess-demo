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
using UnityEngine;

namespace TurboLabz.TLUtils
{
    public static class Flags
    {
        public static Sprite GetFlag(string countryId)
        {
            Sprite flag = null;

            if (countryId != null)
            {
                flag = Resources.Load<Sprite>("Flags/" + countryId);
            }

            if (flag == null)
            {
                flag = Resources.Load<Sprite>("Flags/_Unknown");
            }

            return flag;
        }
    }
}
