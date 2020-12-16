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
    public class CareerCardMediator : Mediator
    {
        // View injection
        [Inject] public CareerCardView view { get; set; }

        //Dispatch signals
        [Inject] public ShowAdSignal showAdSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.OnInfoBtnClickedSignal.AddListener(InfoButtonClicked);
        }

        [ListensTo(typeof(UpdateLeagueProfileSignal))]
        public void UpdateView(string id)
        {
            view.UpdateView();
        }

        public void InfoButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LEAGUE_PERKS_VIEW);
        }
    }
}
