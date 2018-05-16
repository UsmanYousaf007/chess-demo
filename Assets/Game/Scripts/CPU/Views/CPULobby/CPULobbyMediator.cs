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

namespace TurboLabz.InstantChess
{
    public class CPULobbyMediator : Mediator
    {
        // Dispatch signals
        [Inject] public AdjustStrengthSignal adjustStrengthSignal { get; set; }
        [Inject] public AdjustDurationSignal adjustTimerSignal { get; set; }
        [Inject] public AdjustPlayerColorSignal adjustPlayerColorSignal { get; set; }
		[Inject] public AdjustThemeSignal adjustThemeSignal { get; set; }
        [Inject] public StartNewGameSignal startCPUGameSignal { get; set; }
        [Inject] public DevFenValueChangedSignal devFenValueChangedSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public SaveGameSignal saveGameSignal { get; set; }
        [Inject] public LoadStatsSignal loadStatsSignal { get; set; }
		[Inject] public LoadStoreSignal loadStoreSignal { get; set; }
        [Inject] public ShareAppSignal shareAppSignal { get; set; }

        // View injection
        [Inject] public CPULobbyView view { get; set; }

        // Services
        [Inject] public IAndroidNativeService androidNativeService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.decStrengthButtonClickedSignal.AddListener(OnDecStrengthButtonClicked);
            view.incStrengthButtonClickedSignal.AddListener(OnIncStrengthButtonClicked);
            view.decDurationButtonClickedSignal.AddListener(OnDecTimeButtonClicked);
            view.incDurationButtonClickedSignal.AddListener(OnIncTimeButtonClicked);
            view.decPlayerColorButtonClickedSignal.AddListener(OnDecPlayerColorButtonClicked);
            view.incPlayerColorButtonClickedSignal.AddListener(OnIncPlayerColorButtonClicked);
			view.incThemeButtonClickedSignal.AddListener(OnIncThemeButtonClicked);
			view.decThemeButtonClickedSignal.AddListener(OnDecThemeButtonClicked);
            view.playButtonClickedSignal.AddListener(OnPlayButtonClicked);
			view.themesButtonClickedSignal.AddListener(OnThemesButtonClicked);
            view.devFenValueChangedSignal.AddListener(OnDevFenValueChanged);
            view.statsButtonClickedSignal.AddListener(OnStatsButtonClicked);
            view.shareAppButtonClickedSignal.AddListener(OnShareAppButtonClicked);
        }

        public override void OnRemove()
        {
            view.decStrengthButtonClickedSignal.RemoveAllListeners();
            view.incStrengthButtonClickedSignal.RemoveAllListeners();
            view.decDurationButtonClickedSignal.RemoveAllListeners();
            view.incDurationButtonClickedSignal.RemoveAllListeners();
            view.decPlayerColorButtonClickedSignal.RemoveAllListeners();
            view.incPlayerColorButtonClickedSignal.RemoveAllListeners();
            view.playButtonClickedSignal.RemoveAllListeners();
            view.devFenValueChangedSignal.RemoveAllListeners();
            view.statsButtonClickedSignal.RemoveAllListeners();
            view.CleanUp();
        }
            
        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_LOBBY) 
            {
                view.Show();
            }
            else if (viewId == NavigatorViewId.FREE_BUCKS_REWARD_DLG)
            {
                view.ShowFreeBucksRewardDlg();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_LOBBY)
            {
                view.Hide();
                saveGameSignal.Dispatch();
            }
            else if (viewId == NavigatorViewId.FREE_BUCKS_REWARD_DLG)
            {
                view.HideFreeBucksRewardDlg();
            }
        }

        [ListensTo(typeof(GameAppEventSignal))]
        public void OnAppEvent(AppEvent evt)
        {
            if (!view || !view.IsVisible())
            {
                return;
            }

            if (evt == AppEvent.PAUSED || evt == AppEvent.QUIT)
            {
                saveGameSignal.Dispatch();
            }
            else if (evt == AppEvent.ESCAPED)
            {
                androidNativeService.SendToBackground();
            }
        }

        [ListensTo(typeof(UpdateMenuViewSignal))]
        public void OnUpdateView(CPULobbyVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateStrengthSignal))]
        public void OnUpdateStrength(CPULobbyVO vo)
        {
            view.UpdateStrength(vo);
        }

        [ListensTo(typeof(UpdateDurationSignal))]
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

        [ListensTo(typeof(UpdateLobbyAdsSignal))]
        public void OnUpdateAds(AdsVO vo)
        {
            view.UpdateAds(vo);
        }

        [ListensTo(typeof(UpdateFreeBucksRewardSignal))]
        public void OnUpdateFreeBucksRewardSignal(int amt)
        {
            view.UpdateFreeBucksReward(amt);
        }

        private void OnDecStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(false);
        }

        private void OnIncStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(true);
        }

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
			adjustThemeSignal.Dispatch(true);
		}

		private void OnDecThemeButtonClicked()
		{
			adjustThemeSignal.Dispatch(false);
		}

        private void OnIncPlayerColorButtonClicked()
        {
            adjustPlayerColorSignal.Dispatch(true);
        }

        private void OnPlayButtonClicked()
        {
            startCPUGameSignal.Dispatch();
        }

		private void OnThemesButtonClicked()
		{
			loadStoreSignal.Dispatch();
		}

        private void OnDevFenValueChanged(string fen)
        {
            devFenValueChangedSignal.Dispatch(fen);
        }

        private void OnStatsButtonClicked()
        {
            loadStatsSignal.Dispatch();
        }

		private void OnStoreButtonClicked()
		{
			loadStoreSignal.Dispatch ();
		}

		private void OnShareAppButtonClicked()
        {
            shareAppSignal.Dispatch();
        }
    }
}
