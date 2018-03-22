/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-02 13:44:13 UTC+05:00
/// 
/// @description
/// This is a special monobehaviour event handler to be used by service requests
/// which do not have any access to injectable strange signals.

using UnityEngine;

using strange.extensions.signal.impl;

namespace TurboLabz.Common
{
    public class AppEventDispatcher : IAppEventDispatcher
    {
        public Signal appPausedSignal { get; set; }
        public Signal appResumedSignal { get; set; }

        protected AppEventBehavior monoBehavior;

        private const string GAME_OBJECT_NAME = "AppEventDispatcher";

        public AppEventDispatcher()
        {
            GameObject go = GameObject.Find(GAME_OBJECT_NAME);

            if (go == null)
            {
                go = new GameObject(GAME_OBJECT_NAME);
            }

            monoBehavior = go.GetComponent<AppEventBehavior>();

            if (monoBehavior == null)
            {
                monoBehavior = go.AddComponent<AppEventBehavior>();
            }

            InitListeners();
        }

        ~AppEventDispatcher()
        {
            CleanupListeners();
        }

        private void InitListeners()
        {
            appPausedSignal = new Signal();
            appResumedSignal = new Signal();
            monoBehavior.appPausedSignal.AddListener(appPausedSignal.Dispatch);
            monoBehavior.appResumedSignal.AddListener(appResumedSignal.Dispatch);
        }

        private void CleanupListeners()
        {
            monoBehavior.appPausedSignal.RemoveListener(appPausedSignal.Dispatch);
            monoBehavior.appResumedSignal.RemoveListener(appResumedSignal.Dispatch);
        }
    }

    public class AppEventBehavior : MonoBehaviour
    {
        public Signal appPausedSignal = new Signal();
        public Signal appResumedSignal = new Signal();

        void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                appPausedSignal.Dispatch();
            }
            else
            {
                appResumedSignal.Dispatch();
            }
        }
    }
}
