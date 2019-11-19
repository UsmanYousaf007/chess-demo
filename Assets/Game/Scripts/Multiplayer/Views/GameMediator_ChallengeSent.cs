/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public partial class GameMediator
    {
        public void OnRegisterChallengeSent()
        {
            view.InitChallengeSent();

            view.challengeSentBackToLobbyButtonSignal.AddListener(OnChallengeSentBackClicked);
        }

        [ListensTo(typeof(ChallengeAcceptedSignal))]
        public void OnChallengeAccepted()
        {
            view.inputField.enabled = true;
            if (view.challengeSentDialog.activeSelf)
            {
                view.HideChallengeSent();
            }
        }

        private void OnChallengeSentBackClicked()
        {
            view.HideChallengeSent();
        }
    }
}
