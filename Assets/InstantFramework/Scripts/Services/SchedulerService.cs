using System;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public class SchedulerService : ISchedulerService
    {
        public bool running { get; set; }

        [Inject] public IRoutineRunner routineRunner { get; set; }

        private Signal callbackSignal;
        private WaitForSecondsRealtime waitForOneSecond;

        public void Init()
        {
            callbackSignal = new Signal();
            waitForOneSecond = new WaitForSecondsRealtime(1f);
        }

        public void Start()
        {
            running = true;
            routineRunner.StartCoroutine(SchedulerCoroutine());
        }

        public void Stop()
        {
            routineRunner.StopCoroutine(SchedulerCoroutine());
            running = false;
        }

        public void Subscribe(Action callback)
        {
            callbackSignal.AddListener(callback);
        }

        public void UnSubscribe(Action callback)
        {
            callbackSignal.RemoveListener(callback);
        }

        public void Clear()
        {
            callbackSignal.RemoveAllListeners();
        }

        IEnumerator SchedulerCoroutine()
        {
            while (running)
            {
                yield return waitForOneSecond;
                callbackSignal.Dispatch();
            }
        }
    }
}
