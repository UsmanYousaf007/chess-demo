/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TurboLabz.CPU;

namespace TurboLabz.InstantGame
{
    public class LobbyMediator : Mediator
    {
        // Dispatch signals
        [Inject] public AdjustStrengthSignal adjustStrengthSignal { get; set; }
        [Inject] public AdjustDurationSignal adjustTimerSignal { get; set; }
        [Inject] public AdjustPlayerColorSignal adjustPlayerColorSignal { get; set; }
        [Inject] public StartCPUGameSignal startCPUGameSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public DevFenValueChangedSignal devFenValueChangedSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ShowAdSignal showAdSignal { get; set; }
        [Inject] public UpdateAdsSignal updateAdsSignal { get; set; }
        [Inject] public AuthFaceBookSignal authFacebookSignal { get; set; }

        // View injection
        [Inject] public LobbyView view { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.playMultiplayerButtonClickedSignal.AddListener(OnPlayMultiplayerButtonClicked);

            view.playCPUButtonClickedSignal.AddListener(OnPlayCPUButtonClicked);
            view.decStrengthButtonClickedSignal.AddListener(OnDecStrengthButtonClicked);
            view.incStrengthButtonClickedSignal.AddListener(OnIncStrengthButtonClicked);

            /*
            view.decDurationButtonClickedSignal.AddListener(OnDecTimeButtonClicked);
            view.incDurationButtonClickedSignal.AddListener(OnIncTimeButtonClicked);
            view.decPlayerColorButtonClickedSignal.AddListener(OnDecPlayerColorButtonClicked);
            view.incPlayerColorButtonClickedSignal.AddListener(OnIncPlayerColorButtonClicked);
			view.incThemeButtonClickedSignal.AddListener(OnIncThemeButtonClicked);
			view.decThemeButtonClickedSignal.AddListener(OnDecThemeButtonClicked);
			view.themesButtonClickedSignal.AddListener(OnThemesButtonClicked);
   */         
            view.devFenValueChangedSignal.AddListener(OnDevFenValueChanged);
     
            //view.statsButtonClickedSignal.AddListener(OnStatsButtonClicked);

            view.freeBucksButtonClickedSignal.AddListener(OnFreeBucksButtonClicked);
            view.freeBucksUpdateAdsSignal.AddListener(OnUpdateAdsSignal);
        }

        public override void OnRemove()
        {
            view.decStrengthButtonClickedSignal.RemoveAllListeners();
            view.incStrengthButtonClickedSignal.RemoveAllListeners();
            view.decDurationButtonClickedSignal.RemoveAllListeners();
            view.incDurationButtonClickedSignal.RemoveAllListeners();
            view.decPlayerColorButtonClickedSignal.RemoveAllListeners();
            view.incPlayerColorButtonClickedSignal.RemoveAllListeners();
            view.playCPUButtonClickedSignal.RemoveAllListeners();
            view.devFenValueChangedSignal.RemoveAllListeners();
            view.statsButtonClickedSignal.RemoveAllListeners();

            view.freeBucksButtonClickedSignal.RemoveAllListeners();
            view.freeBucksRewardOkButtonClickedSignal.RemoveAllListeners();
            view.freeBucksUpdateAdsSignal.RemoveAllListeners();
            view.CleanUp();
        }
            
        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateMenuViewSignal))]
        public void OnUpdateView(LobbyVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateStrengthSignal))]
        public void OnUpdateStrength(LobbyVO vo)
        {
            view.UpdateStrength(vo);
        }

        [ListensTo(typeof(UpdateLobbyAdsSignal))]
        public void OnUpdateAds(AdsVO vo)
        {
            view.UpdateAds(vo);
        }

        private void OnDecStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(false);
        }

        private void OnIncStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(true);
        }

        /*
        private void OnDecTimeButtonClicked()
        {
            adjustTimerSignal.Dispatch(false);
        }

        private void OnIncTimeButtonClicked()
        {
            adjustTimerSignal.Dispatch(true);
        }

        private void OnDecPlayerColorButtonClicked()
        {
            adjustPlayerColorSignal.Dispatch(false);
        }

		private void OnIncThemeButtonClicked()
		{
			//adjustThemeSignal.Dispatch(true);
		}

		private void OnDecThemeButtonClicked()
		{
			//adjustThemeSignal.Dispatch(false);
		}

        private void OnIncPlayerColorButtonClicked()
        {
            adjustPlayerColorSignal.Dispatch(true);
        }
        */

        private void OnPlayCPUButtonClicked()
        {
            startCPUGameSignal.Dispatch();
        }

        private void OnPlayMultiplayerButtonClicked()
        {
            findMatchSignal.Dispatch();
        }

        /*
		private void OnThemesButtonClicked()
		{
			loadStoreSignal.Dispatch();
		}
*/
        private void OnDevFenValueChanged(string fen)
        {
            devFenValueChangedSignal.Dispatch(fen);
        }
        /*
        private void OnStatsButtonClicked()
        {
            loadStatsSignal.Dispatch();
        }

		private void OnStoreButtonClicked()
		{
			loadStoreSignal.Dispatch ();
		}
  */      

		private void OnFacebookButtonClicked()
        {
            authFacebookSignal.Dispatch();
        }

        private void OnFreeBucksButtonClicked()
        {
            showAdSignal.Dispatch(true);
        }
 
        private void OnUpdateAdsSignal()
        {
            updateAdsSignal.Dispatch();
        }
    }
}

/*
 * [ListensTo(typeof(UpdateDurationSignal))]
        public void OnUpdateDuration(CPULobbyVO vo)
        {
            view.UpdateDuration(vo);
        }

        [ListensTo(typeof(UpdatePlayerColorSignal))]
        public void OnUpdatePlayerColor(CPULobbyVO vo)
        {
            view.UpdatePlayerColor(vo);
        }

        [ListensTo(typeof(UpdateThemeSignal))]
        public void OnThemePlayerColor(CPULobbyVO vo)
        {
            view.UpdateTheme(vo);
        }
        */