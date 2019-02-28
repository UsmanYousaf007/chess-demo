/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.CPU
{
    public class ToggleSafeModeCommand : Command
    {

        // Dispatch Signals
        [Inject] public UpdateSafeMoveStateSignal updateSafeMoveStateSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void Execute()
        {
            chessboardModel.inSafeMode = !chessboardModel.inSafeMode;
            updateSafeMoveStateSignal.Dispatch(chessboardModel.inSafeMode);
            preferencesModel.isSafeMoveOn = chessboardModel.inSafeMode;

            // Analytics
            if (chessboardModel.inSafeMode)
            {
                analyticsService.Event(AnalyticsEventId.tap_pow_safe_move_on, AnalyticsContext.computer_match);
            }
            else
            {
                analyticsService.Event(AnalyticsEventId.tap_pow_safe_move_off, AnalyticsContext.computer_match);
            }
        }
    }
}
