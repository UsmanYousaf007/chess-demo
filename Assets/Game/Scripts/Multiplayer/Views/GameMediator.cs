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

namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator : Mediator
    {
        // View injection
        [Inject] public GameView view { get; set; }

        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public StopTimersSignal stopTimersSignal { get; set; }
        [Inject] public ExitLongMatchSignal exitLongMatchSignal { get; set; }

        public override void OnRegister()
        {
            OnRegisterChessboard();
            OnRegisterClock();
            OnRegisterPromotions();
            OnRegisterResults();
            OnRegisterScore();
            OnRegisterMenu();
            OnRegisterFind();
            OnRegisterDraw();
            OnRegisterWifi();
            OnRegisterAccept();
            OnRegisterBotBar();
            OnRegisterChat();
            OnRegisterSafeMove();
            OnRegisterHint();
            OnRegisterHindsight();
            OnRegisterInfo();
            OnRegisterSpotPurchase();
            OnRegisterChallengeSent();
        }

        public override void OnRemove()
        {
            OnRemoveChessboard();
            OnRemoveClock();
            OnRemovePromotions();
            OnRemoveResults();
            OnRemoveScore();
            OnRemoveMenu();
            OnRemoveDraw();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER) 
            {
                view.Show();
                InternetReachabilityMonitor.AddListener(OnInternetConnectedTicked);
                InternetReachabilityMonitor.AddSlowInternetListener(OnSlowInternet);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER)
            {
                InternetReachabilityMonitor.RemoveListener(OnInternetConnectedTicked);
                InternetReachabilityMonitor.RemoveSlowInternetListener(OnSlowInternet);
                stopTimersSignal.Dispatch();
                view.Hide();
            }
        }

        [ListensTo(typeof (PreShowNotificationSignal))]
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

        [ListensTo(typeof(OpponentPingedForConnectionSignal))]
        public void OnOpponentPingedForConnection(bool isAck)
        {
            if (gameObject.activeSelf)
            {
                view.EnableOpponentConnectionMonitor(!isAck);
            }
        }
    }
}
