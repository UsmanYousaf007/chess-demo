/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-08-01 14:55:05 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Chess
{
    public class CDSPlayerTurnSamePieceGrabbed : CDS
    {
        public override CDS HandleEvent(CDSEvent evt, CDSModel model)
        {
            if (evt == CDSEvent.MOUSE_DRAG)
            {
                return new CDSPlayerTurnPieceDragging();
            }
            else if (evt == CDSEvent.MOUSE_UP)
            {
                model.squareClickedSignal.Dispatch(model.mouseDownLocation);
            }
            else if (evt == CDSEvent.PLAYER_TURN_PIECE_DESELECTED)
            {
                return new CDSPlayerTurnNothingSelected();
            }

            return null;
        }
    }
}
