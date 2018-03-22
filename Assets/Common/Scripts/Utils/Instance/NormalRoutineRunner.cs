/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-22 16:35:07 UTC+05:00
/// 
/// @description
/// A routine runner that can be used by non-strange objects

using UnityEngine;

namespace TurboLabz.Common
{
    public class NormalRoutineRunner : RoutineRunner
    {
        private const string GAME_OBJECT_NAME = "NormalRoutineRunner";

        public NormalRoutineRunner() : base()
        {
            GameObject go = GameObject.Find(GAME_OBJECT_NAME);

            if (go == null)
            {
                go = new GameObject(GAME_OBJECT_NAME);
            }

            monoBehavior = go.GetComponent<RoutineRunnerBehavior>();

            if (monoBehavior == null)
            {
                monoBehavior = go.AddComponent<RoutineRunnerBehavior>();
            }
        }
    }
}
