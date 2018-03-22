/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-23 15:36:56 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections;

using UnityEngine;

namespace TurboLabz.Common
{
    public class RoutineRunner : IRoutineRunner
    {
        protected RoutineRunnerBehavior monoBehavior;

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return monoBehavior.StartCoroutine(routine);
        }

        public void StopCoroutine(IEnumerator routine)
        {
            monoBehavior.StopCoroutine(routine);
            routine = null; // TODO: find out if we really need to null it here
        }
    }

    public class RoutineRunnerBehavior : MonoBehaviour
    {
    }
}
