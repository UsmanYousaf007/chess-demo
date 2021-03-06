/// @license #LICENSE# <#LICENSE_URL#>
/// @copyright Copyright (C) #COMPANY# #YEAR# - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author #AUTHOR# <#AUTHOR_EMAIL#>
/// @company #COMPANY# <#COMPANY_URL#>
/// @date #DATE#
/// 
/// @description
/// #DESCRIPTION#
using System;

namespace TurboLabz.Chess
{
    public struct AiMoveInputVO
    {
        public ChessColor aiColor;
        public ChessColor playerColor;
        public ChessMove lastPlayerMove;
        public ChessSquare[,] squares;
        public TimeSpan opponentTimer;
        public AiMoveDelay aiMoveDelay;
        public int aiMoveNumber;
        public bool isHint;
        public float cpuStrengthPct;
        public bool isStrength;
        public float playerStrengthPct;
        public string fen;
        public bool isBot;
        public bool analyse;
    }
}
