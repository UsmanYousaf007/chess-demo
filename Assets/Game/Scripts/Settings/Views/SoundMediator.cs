/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

using strange.extensions.mediation.impl;
using System.Collections.Generic;
using TurboLabz.Multiplayer;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System;

namespace TurboLabz.InstantFramework
{
    public class SoundMediator : Mediator
    {
        // View injection
        [Inject] public SoundView view { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }


        public override void OnRegister()
        {
            view.Init();

            
        }
    }
}

