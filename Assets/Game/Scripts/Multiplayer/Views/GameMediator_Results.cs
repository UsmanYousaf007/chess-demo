/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:03:30 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Gamebet;
using TurboLabz.Chess;
using TurboLabz.Common;

namespace TurboLabz.MPChess 
{
    public partial class GameMediator
    {
        [Inject] public LoadEndGameSignal loadEndGameSignal { get; set; }

        public void OnRegisterResults()
        {
            view.viewDurationCompleteSignal.AddListener(OnViewDurationComplete);
            view.InitResults();
        }

        public void OnRemoveResults()
        {
            view.viewDurationCompleteSignal.RemoveListener(OnViewDurationComplete);
            view.CleanupResults();
        }

        [ListensTo(typeof(ShowResultsDialogSignal))]
        public void OnUpdateResults(GameEndReason gameEndReason, bool playerWins)
        {
            view.ShowResultsDialog(gameEndReason, playerWins);
        }

        private void OnViewDurationComplete()
        {
            loadEndGameSignal.Dispatch();
        }
    }
}
