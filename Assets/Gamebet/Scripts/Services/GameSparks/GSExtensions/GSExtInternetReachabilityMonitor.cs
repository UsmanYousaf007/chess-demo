/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-21 22:49:36 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections;

using UnityEngine;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class GSExtInternetReachabilityMonitor
    {
        private enum State
        {
            STOPPED = -1,
            MONITORING
        }

        // Utils
        private IRoutineRunner routineRunner = new NormalRoutineRunner();
        private IGameEngineInfo gameEngineInfo = new UnityInfo();

        private Action onFailure;
        private IEnumerator startCR;

        private State state = State.STOPPED;

        public void Start(Action onFailure)
        {
            Assertions.Assert(state == State.STOPPED, "The Internet Reachability Monitor must not already be running!");

            state = State.MONITORING;
            this.onFailure = onFailure;
            startCR = StartCR();
            routineRunner.StartCoroutine(startCR);
        }

        public void Stop()
        {
            Assertions.Assert(state == State.MONITORING, "The Internet Reachability Monitor must already be running!");

            routineRunner.StopCoroutine(startCR);
            startCR = null;
            state = State.STOPPED;
        }

        private IEnumerator StartCR()
        {
            while (true)
            {
                if (!gameEngineInfo.isInternetReachable)
                {
                    onFailure();
                    break;
                }

                yield return new WaitForSecondsRealtime(GSSettings.INTERNET_REACHABLITY_MONITOR_FREQUENCY);
            }
        }
    }
}
