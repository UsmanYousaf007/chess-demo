/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

namespace SocialEdge.Utils
{
    public class SocialEgeRunnerBehavior : MonoBehaviour { }

    public class SocialEdgeRoutineRunner
    {
        private const string GAME_OBJECT_NAME = "SocialEdgeRoutineRunner";
        public MonoBehaviour monoBehavior;

        public SocialEdgeRoutineRunner()
        {
            GameObject go = GameObject.Find(GAME_OBJECT_NAME);

            if (go == null)
            {
                go = new GameObject(GAME_OBJECT_NAME);
            }

            monoBehavior = go.GetComponent<SocialEgeRunnerBehavior>();

            if (monoBehavior == null)
            {
                monoBehavior = go.AddComponent<SocialEgeRunnerBehavior>();
            }
        }
    }
}
