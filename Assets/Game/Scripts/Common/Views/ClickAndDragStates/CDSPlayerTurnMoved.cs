/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-08-01 14:54:49 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

namespace TurboLabz.Chess
{
    public class CDSPlayerTurnMoved : CDS
    {
        // Render dragging
        public override void RenderDisplayOnEnter(CDSModel model)
        {
            model.squareClickedSignal.Dispatch(model.mouseDragLocation);
        }

        public override CDS HandleEvent(CDSEvent evt, CDSModel model)
        {
            if (evt == CDSEvent.OPPONENT_TURN)
            {
                return new CDSOpponentTurn();
            }

            return null;
        }
    }
}
