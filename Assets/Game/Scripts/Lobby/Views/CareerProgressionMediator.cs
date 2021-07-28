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
    public class CareerProgressionMediator : Mediator
    {
        // View injection
        [Inject] public CareerProgressionView view { get; set; }

        //Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateTrophyBarSignal updateTrophyBarSignal { get; set; }
        [Inject] public ShowLobbyRewardsSignal showLobbyRewardsSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.loadLobby.AddListener(OnLoadLobbySignal);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CAREER_PROGRESSION_DLG)
            {
                view.ShowCareerProgression();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CAREER_PROGRESSION_DLG)
            {
                view.Hide();
            }
        }

        /*[ListensTo(typeof(UpdateCareerCardSignal))]
        public void UpdateView(CareerCardVO vo)
        {
            view.UpdateView(vo);
        }*/

        [ListensTo(typeof(ResetCareerprogressionViewSignal))]
        public void Reset()
        {
            view.Reset();
        }

        private void OnLoadLobbySignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);
            showLobbyRewardsSignal.Dispatch();
            updateTrophyBarSignal.Dispatch();
        }
    }
}
