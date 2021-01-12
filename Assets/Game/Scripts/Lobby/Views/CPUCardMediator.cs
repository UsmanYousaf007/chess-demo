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
    public class CPUCardMediator : Mediator
    {
        // View injection
        [Inject] public CPUCardView view { get; set; }

        //Dispatch signals
        [Inject] public AdjustStrengthSignal adjustStrengthSignal { get; set; }
        [Inject] public StartCPUGameSignal startCPUGameSignal { get; set; }
        [Inject] public ShowAdSignal showAdSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        // Services
        [Inject] public IPreGameAdsService preGameAdsService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.playCPUButtonClickedSignal.AddListener(OnPlayComputerMatchBtnClicked);
            view.decStrengthButtonClickedSignal.AddListener(OnDecStrengthButtonClicked);
            view.incStrengthButtonClickedSignal.AddListener(OnIncStrengthButtonClicked); 
        }

        private void OnDecStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(false);
        }

        private void OnIncStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(true);
        }

        [ListensTo(typeof(UpdateStrengthSignal))]
        public void OnUpdateStrength(LobbyVO vo)
        {
            view.UpdateStrength(vo);
        }

        private void OnPlayComputerMatchBtnClicked()
        {
            if (cpuGameModel.inProgress)
            {
                preGameAdsService.ShowPreGameAd().Then(() => startCPUGameSignal.Dispatch(false));
            }
            else
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU_POWER_MODE);
            }
        }

        [ListensTo(typeof(UpdateMenuViewSignal))]
        public void OnUpdateView(LobbyVO vo)
        {
            view.UpdateView(vo);
        }
    }
}