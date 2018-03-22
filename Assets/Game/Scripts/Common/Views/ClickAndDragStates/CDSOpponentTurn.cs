/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-08-01 14:53:24 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

namespace TurboLabz.Chess
{
    public class CDSOpponentTurn : CDS
    {
        public override CDS HandleEvent(CDSEvent evt, CDSModel model)
        {
            if (evt == CDSEvent.MOUSE_DOWN)
            {
                model.squareClickedSignal.Dispatch(model.mouseDownLocation);
            }
            else if (evt == CDSEvent.PLAYER_TURN)
            {
                return new CDSPlayerTurnNothingSelected();
            }
            else if (evt == CDSEvent.PLAYER_TURN_PIECE_SELECTED)
            {
                if (Input.GetMouseButton(0))
                {
                    return new CDSPlayerTurnNewPieceGrabbed();
                }
                else
                {
                    return new CDSPlayerTurnPieceSelected();
                }
            }
            else if (evt == CDSEvent.PLAYER_TURN)
            {
                return new CDSPlayerTurnNothingSelected();
            }

            return null;
        }
    }
}

