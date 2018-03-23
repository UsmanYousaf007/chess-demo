/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-14 12:58:58 UTC+05:00
/// 
/// @description
/// This is a common service/model pattern in Strange:
/// We want something usually reserved to MonoBehaviours to be available
/// elsewhere. Maybe someday we'll write a version that
/// eschews MonoBehaviours altogether...but for now we simply leverage
/// that behavior and provide it in injectable form.
/// 
/// In this case, we're making Coroutines available everywhere in the app
/// by attaching a MonoBehaviour to the ContextView.
/// 
/// IRoutineRunner can be injected anywhere, minimizing direct dependency
/// on MonoBehaviours.

using UnityEngine;

using strange.extensions.context.api;

namespace TurboLabz.TLUtils
{
    public class StrangeRoutineRunner : RoutineRunner
    {
        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject contextView{ get; set; }

        public StrangeRoutineRunner() : base() {}

        [PostConstruct]
        public void PostConstruct()
        {
            monoBehavior = contextView.GetComponent<RoutineRunnerBehavior>();

            if (monoBehavior == null)
            {
                monoBehavior = contextView.AddComponent<RoutineRunnerBehavior>();
            }
        }
    }
}
