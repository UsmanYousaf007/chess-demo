/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-21 21:44:37 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections;

using UnityEngine;

using GameSparks.Core;

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class GSExtConnectRequest
    {
        // Utils
        private IRoutineRunner routineRunner = new NormalRoutineRunner();

        private Action onSuccess;
        private Action onFailure;

        public void Send(Action onSuccess, Action onFailure, bool isGSInitialized)
        {
            this.onSuccess = onSuccess;
            this.onFailure = onFailure;
            routineRunner.StartCoroutine(SendCR(isGSInitialized));
        }

        private IEnumerator SendCR(bool isGSInitialized)
        {
            // Wait for one frame to ensure that this call is asynchronous
            // as required by the Promise pattern.
            yield return null;

            // You cannot call GS.Reconnect() until GameSparks is initalized and
            // that happens automatically when the app launches because the
            // GameSparks SDK object is in the scene heirarchy. Note that during
            // app launch the GS initialization also begins an auto connect and
            // therefore this call below won't work in that scenario.
            if (isGSInitialized)
            {
                //GS.Reconnect();
            }
            else
            {
                //GS.Initialise(GS.GSPlatform);
            }

            routineRunner.StartCoroutine(WaitForConnectCR());
        }

        private IEnumerator WaitForConnectCR()
        {
            float startTime = Time.realtimeSinceStartup;

            while (true)
            {
                if (GS.Available)
                {
                    onSuccess();
                    break;
                }

                float elapsedTime = Time.realtimeSinceStartup - startTime;

                if (elapsedTime > GSSettings.GS_CONNECT_TIMEOUT)
                {
                    onFailure();
                    break;
                }

                yield return new WaitForSecondsRealtime(GSSettings.GS_CONNECT_CHECK_FREQUENCY);
            }
        }
    }
}
