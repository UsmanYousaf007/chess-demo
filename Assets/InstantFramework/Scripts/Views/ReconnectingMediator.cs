/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 18:53:20 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class ReconnectingMediator : Mediator
    {
        // View injection
        [Inject] public ReconnectingView view { get; set; }

        // Dispatch signals
        [Inject] public SetErrorAndHaltSignal setErrorAndHaltSignal { get; set; }

        // Dispatch signals
        [Inject] public ToggleBannerSignal toggleBannerSignal { get; set; }
        [Inject] public PauseNotificationsSignal pauseNotificationsSignal { get; set; }
        [Inject] public ReconnectResetStateSignal reconnectResetStateSignal { get; set; }
        [Inject] public SignalLostSaveStateSignal signalLostSaveStateSignal { get; set; }

        // Models
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            backendService.OnlineCheckerAddListener(OnInternetConnectedTicked);
            gameObject.SetActive(true);
            view.HidePopUp();
        }

        [ListensTo(typeof(RequestToggleBannerSignal))]
        public void OnRequestToggleBannerSignal()
        {
            if (appInfoModel.isReconnecting == DisconnectStates.FALSE
                && appInfoModel.gameMode != GameMode.NONE
                && !appInfoModel.isNotificationActive) 
            {
                toggleBannerSignal.Dispatch(true);
            }
        }

        [ListensTo(typeof(ChessboardBlockerEnableSignal))]
        public void OnUIBlockerEnable(bool enable)
        {
            view.uiBlocker.SetActive(enable);
        }

        [ListensTo(typeof(ReconnectViewEnableSignal))]
        public void OnReconnectEnable(bool enable)
        {
            if(enable)
            {
                toggleBannerSignal.Dispatch(false);
                signalLostSaveStateSignal.Dispatch();
                view.ShowPopUp();
            }
            else
            {
                view.HidePopUp();
                reconnectResetStateSignal.Dispatch();
            }
        }

        private void OnInternetConnectedTicked(bool isConnected, ConnectionSwitchType connectionSwitch)
        {
            if (appInfoModel.gameMode == GameMode.NONE || matchInfoModel.activeChallengeId == null)
            {
                if (isConnected)
                {
                    view.HidePopUp();
                }
                else
                {
                    analyticsService.Event(AnalyticsEventId.reconnection_shown, AnalyticsContext.internet_disconnect);
                    view.ShowPopUp();
                }
            }

            if (connectionSwitch == ConnectionSwitchType.FROM_CONNECTED_TO_DISCONNECTED)
            {
                // Make sure state is not in long disconnect
                if (appInfoModel.isReconnecting == DisconnectStates.FALSE)
                {
                    appInfoModel.isReconnecting = DisconnectStates.SHORT_DISCONNECT;
                    backendService.StopPinger();
                }

                toggleBannerSignal.Dispatch(false);
                pauseNotificationsSignal.Dispatch(true);
            }
            else
            if (connectionSwitch == ConnectionSwitchType.FROM_DISCONNECTED_TO_CONNECTED)
            {
                if (appInfoModel.isReconnecting == DisconnectStates.SHORT_DISCONNECT)
                {
                    appInfoModel.isReconnecting = DisconnectStates.FALSE;
                    //GameSparks.Core.GS.Reconnect();
                    backendService.StartPinger();
                }

                // Switch on banner ads on reconnection when on chess board
                if (navigatorModel.currentViewId == NavigatorViewId.MULTIPLAYER || 
                    navigatorModel.currentViewId == NavigatorViewId.CPU ||
                    navigatorModel.currentViewId == NavigatorViewId.MULTIPLAYER_CHAT_DLG)
                {
                    toggleBannerSignal.Dispatch(true);
                }

                pauseNotificationsSignal.Dispatch(false);

                Crosstales.OnlineCheck.OnlineCheck.SetNormalState();
            }
        }
    }
}
