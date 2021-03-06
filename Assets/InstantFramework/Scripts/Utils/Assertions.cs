/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-24 20:02:00 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Diagnostics;

namespace TurboLabz.TLUtils
{
    public static class Assertions
    {
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, string message)
        {
            // Using the fully qualified name because individual name conflict
            // with our own class and method names i.e. Assertions.Assert
            UnityEngine.Assertions.Assert.IsTrue(condition, message);
        }
    }
}
