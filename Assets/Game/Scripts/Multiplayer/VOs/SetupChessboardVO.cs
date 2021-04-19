using System;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public struct SetupChessboardVO
    {
        public bool isPlayerWhite;
        public bool isLongPlay;
        public bool isRanked;
        public bool powerMode;
        //public bool isTenMinGame;
        //public bool isOneMinGame;
        //public bool isThirtyMinGame;
        public GameTimeMode gameTimeMode;
        public string challengeId;
    }
}

