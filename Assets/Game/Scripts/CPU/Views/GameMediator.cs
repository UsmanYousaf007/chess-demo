/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:45:03 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;

namespace TurboLabz.CPU 
{
    public partial class GameMediator : Mediator
    {
        // View injection
        [Inject] public GameView view { get; set; }

        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public StopTimersSignal stopTimersSignal { get; set; }
        [Inject] public SaveGameSignal saveGameSignal { get; set; }
        [Inject] public PauseTimersSignal pauseTimersSignal { get; set; }
        [Inject] public ResumeTimersSignal resumeTimersSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public LoadSpotInventorySignal loadSpotInventorySignal { get; set; }

        [Inject] public IBackendService backendService { get; set; }

        public override void OnRegister()
        {
            OnRegisterChessboard();
            OnRegisterClock();
            OnRegisterPromotions();
            OnRegisterResults();
            OnRegisterScore();
            OnRegisterMatchInfo();
            OnRegisterMenu();
            OnRegisterSafeMove();
            OnRegisterHint();
            OnRegisterHindsight();
            OnRegisterInfo();
            OnRegisterStep();
            OnRegisterSpecialHint();
            OnRegisterPowerplay();
        }

        public override void OnRemove()
        {
            OnRemoveChessboard();
            OnRemoveClock();
            OnRemovePromotions();
            OnRemoveResults();
            OnRemoveScore();
            OnRemoveMenu();
            OnRemovePowerplay();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU) 
            {
                view.Show();
                view.SetupPowerplayImage(chessboardModel.powerMode);
                view.SetupStepButtons();
                backendService.OnlineCheckerAddListener(OnInternetConnectedTicked);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU)
            {
                stopTimersSignal.Dispatch();
                view.Hide();
                backendService.OnlineCheckerRemoveListener(OnInternetConnectedTicked);
            }
        }

        [ListensTo(typeof(ModelsSaveToDiskSignal))]
        public void OnSaveToDisk()
        {
            if (!view || !view.IsVisible())
            {
                return;
            }

            saveGameSignal.Dispatch();
        }

        [ListensTo(typeof(PreShowNotificationSignal))]
        public void OnPreShowNotification()
        {
            if (gameObject.activeSelf)
            {
                view.OnParentHideAdBanner();
            }
        }

        [ListensTo(typeof(PostShowNotificationSignal))]
        public void OnPostShowNotification()
        {
            if (gameObject.activeSelf)
            {
                view.OnParentShowAdBanner();
            }
        }

        private void OnInternetConnectedTicked(bool isConnected, ConnectionSwitchType connectionSwitch)
        {
            if (connectionSwitch == ConnectionSwitchType.FROM_CONNECTED_TO_DISCONNECTED)
            {
                LogUtil.Log("CPU Match Disconnected", "cyan");
            }
            else
            if (connectionSwitch == ConnectionSwitchType.FROM_DISCONNECTED_TO_CONNECTED)
            {
                if (view.gameObject.activeSelf)
                {
                    LogUtil.Log("CPU Match Connected", "cyan");
                }
            }
        }

        [ListensTo(typeof(DisableModalBlockersSignal))]
        public void OnDisableModalBlockers()
        {
            if (view.IsVisible())
            {
                view.DisableModalBlocker();
            }
        }

    }
}
