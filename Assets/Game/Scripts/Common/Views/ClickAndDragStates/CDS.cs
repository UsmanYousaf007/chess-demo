/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-08-01 14:12:21 UTC+05:00
/// 
/// @description
/// https://docs.google.com/drawings/d/1y7cDZxbTC2IPSr_c3nmdUHkVM5gyUy88Cg76jOOWmpY/edit

using System;
using UnityEngine;

namespace TurboLabz.Chess
{
    public class CDS
    {
        public virtual void RenderDisplayOnEnter(CDSModel model)
        {   
        }

        public virtual CDS HandleEvent(CDSEvent evt, CDSModel model)
        {
            return null;
        }

        ////////////////////////////////////////////////////////////////////////
        // Common state routines

        protected bool IsValidMoveLocation(FileRank location, CDSModel model)
        {
            foreach (ChessSquare square in model.validSquares)
            {   
                if (square.fileRank == location)
                {
                    return true;
                }
            }

            return false;
        }

        protected bool CameFromState(CDSModel model, Type state)
        {
            return (model.previousState.GetType() == state);
        }
    }
}
