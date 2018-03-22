/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-13 04:59:34 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;

using UnityEngine.UI;

using strange.extensions.mediation.impl;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class DebugView : View
    {
        [Inject] public IBackendService backendService { get; set; }

        public Text serverClock;
        public Text latency;

        public void Init()
        {
            // TODO: set language strings
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        void Update()
        {
            DateTime serverClockDt = TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp);
            serverClock.text = String.Format("{0:d/M/yyyy HH:mm:ss}", serverClockDt);
            latency.text = backendService.serverClock.latency + " ms";
        }
    }
}
