/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System.Text;

namespace TurboLabz.InstantFramework
{
    public class ShowProfileDialogCommand : Command
    {
        // Signal parameters
        [Inject] public string opponentId { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateProfileDialogSignal updateProfileDialogSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_PROFILE_DLG);

            LogUtil.Log("Show profile dialog for.." + opponentId);
        }
    }
}
