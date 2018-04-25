﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:03:07 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Chess;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess
{
    public partial class GameMediator
    {
        [ListensTo(typeof(TurnSwapSignal))]
        public void OnToggleThink(bool isPlayerTurn)
        {
            view.ToggleThink(isPlayerTurn);
        }
    }
}
