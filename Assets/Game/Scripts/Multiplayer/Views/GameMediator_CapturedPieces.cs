/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public partial class GameMediator
    {
        [ListensTo(typeof(ResetCapturedPiecesSignal))]
        public void OnResetCapturedPIeces()
        {
            view.ResetCapturedPieces();
        }
    }
}
