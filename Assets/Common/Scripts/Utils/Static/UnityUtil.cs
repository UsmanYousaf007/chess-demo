/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-18 15:16:43 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

namespace TurboLabz.Common
{
    public static class UnityUtil
    {
        /// <summary>
        /// Calls GameObject.Destroy on all children of transform and
        /// immediately detaches the children from transform so after this call
        /// tranform.childCount is zero.
        /// </summary>
        public static void DestroyChildren(Transform transform)
        {
            DestroyChildren(transform, 0, 0);
            transform.DetachChildren();
        }

        public static void DestroyChildren(Transform transform, int omitStart)
        {
            DestroyChildren(transform, omitStart, 0);
        }

        public static void DestroyChildren(Transform transform, int omitStart, int omitEnd)
        {
            int destroyCount = transform.childCount - (omitStart + omitEnd);

            Assertions.Assert(destroyCount >= 0, "The children to omit are greater than the total children in parent!");

            for (int i = transform.childCount - 1 - omitEnd; i >= omitStart; --i)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
