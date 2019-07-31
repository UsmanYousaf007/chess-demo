/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-22 12:55:25 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.TLUtils;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public GameDisconnectingSignal gameDisconnectingSignal { get; set; }
        [Inject] public AppEventSignal appEventSignal { get; set;  }
        [Inject] public ModelsSaveToDiskSignal modelsSaveToDiskSignal { get; set; }
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }
        [Inject] public ModelsLoadFromDiskSignal modelsLoadFromDiskSignal { get; set; }
        [Inject] public ResumeMatchSignal resumeMatchSignal { get; set; }

        [Inject] public ChessboardBlockerEnableSignal chessboardBlockerEnableSignal { get; set; }
        [Inject] public ReconnectViewEnableSignal reconnectViewEnableSignal { get; set; }

        private NavigatorViewId prevViewId;

        public void MonitorConnectivity(bool enable)
        {
            GS.GameSparksAvailable -= GameSparksAvailable;

            if (enable)
            {
                GS.GameSparksAvailable += GameSparksAvailable;
            }
        }

        void GameSparksAvailable(bool isAvailable)
        {
            if (isAvailable)
            {
                LogUtil.Log("GS Connected", "red");

                appInfoModel.isReconnecting = DisconnectStats.FALSE;

                // Reset all models
                modelsResetSignal.Dispatch();
                // Load saved models (perfs etc)
                modelsLoadFromDiskSignal.Dispatch();
                // Restart the reachability monitor
                //InternetReachabilityMonitor.EnableDispatches(true);
                InternetReachabilityMonitor.StartMonitor();
                // Begin processing hard reconnect
                resumeMatchSignal.Dispatch(prevViewId);
                // Start the pinger
                StartPinger();

                chessboardBlockerEnableSignal.Dispatch(false);

            }
            else
            {
                LogUtil.Log("GS Disconnected", "red");

                if(appInfoModel.isReconnecting == DisconnectStats.FALSE)
                {
                    appInfoModel.reconnectTimeStamp = TimeUtil.unixTimestampMilliseconds;
                }

                appInfoModel.isReconnecting = DisconnectStats.LONG_DISCONNET;
                reconnectViewEnableSignal.Dispatch(true);

                chessboardBlockerEnableSignal.Dispatch(true);

                // Stop the pinger
                StopPinger();
                // Avoid soft reconnect processing
                //InternetReachabilityMonitor.EnableDispatches(false);
                InternetReachabilityMonitor.StopMonitor();
                // Reconnect processing depends on last view
                prevViewId = navigatorModel.currentViewId;
                // Remove pending requests processing
                GSFrameworkRequest.CancelRequestSession();
                // Dispatch signal that we are in reconnection
                gameDisconnectingSignal.Dispatch();
                // Save models to disk to reload when coming back from background
                modelsSaveToDiskSignal.Dispatch();
                // Avoid processing gs messages that arrive right after gs becomes available
                // because models will be reset and those messages will no longer be valid. The
                // state will be obtained from full init data.
                RemoveChallengeListeners();
            }
        }
    }
}
