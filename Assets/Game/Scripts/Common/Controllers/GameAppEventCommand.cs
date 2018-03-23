/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-17 15:42:00 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

using TurboLabz.InstantFramework;
using TurboLabz.Chess;

namespace TurboLabz.InstantFramework
{
    public class GameAppEventCommand : Command
    {
        // Signal parameters
        [Inject] public AppEvent appEvent { get; set; }

        // Models
        [Inject] public IChessAiService chessAiService { get; set; }

        public override void Execute()
        {
            if (appEvent == AppEvent.QUIT)
            {
                #if UNITY_EDITOR
                if (chessAiService.isInitialized)
                {
                    chessAiService.Shutdown();

                }
                #endif
            }
        }
    }
}
