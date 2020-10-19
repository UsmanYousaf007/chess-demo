﻿using System;

namespace TurboLabz.InstantFramework
{
    public struct ResultAdsVO
    {
        public AdType adsType;
        public string rewardType;
        public string challengeId;
        public bool playerWins;
        public string actionCode;
        public string friendId;
        public bool isRanked;
        public string tournamentId;

        public Action OnAdCompleteCallback;

        public void RemoveCallback()
        {
            OnAdCompleteCallback = null;
        }
    }
}

