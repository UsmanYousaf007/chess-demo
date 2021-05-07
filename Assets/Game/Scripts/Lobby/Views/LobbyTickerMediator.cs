/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System.Collections;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class LobbyTickerMediator : Mediator
    {
        // View injection
        [Inject] public LobbyTickerView view { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
        }

        [ListensTo(typeof(UpdateLeagueProfileSignal))]
        public void UpdateView(string id)
        {
            //view.UpdateView();
        }

        [ListensTo(typeof(UpdateMenuViewSignal))]
        public void OnUpdateView(LobbyVO vo)
        {
            view.UpdateView(vo);
        }
    }
}
