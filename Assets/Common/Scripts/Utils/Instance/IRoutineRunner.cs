/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-14 12:58:03 UTC+05:00
/// 
/// @description
/// Interface for a service.
/// In this case, the service allows us to run a Coroutine outside
/// the strict confines of a MonoBehaviour. (See RoutineRunner)

using UnityEngine;
using System.Collections;

namespace TurboLabz.Common
{
    public interface IRoutineRunner
    {
        Coroutine StartCoroutine(IEnumerator method);
        void StopCoroutine(IEnumerator routine);
    }
}
