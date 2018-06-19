/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-08-02 12:54:55 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.CPU 
{
    public partial class GameMediator
    {
        [ListensTo(typeof(EnablePlayerTurnInteractionSignal))]
        public void OnEnablePlayerTurnInteraction()
        {
            view.EnablePlayerTurnInteraction();
        }

        [ListensTo(typeof(EnableOpponentTurnInteractionSignal))]
        public void OnEnableOpponentTurnInteraction()
        {
            view.EnableOpponentTurnInteraction();
        }
    }
}
