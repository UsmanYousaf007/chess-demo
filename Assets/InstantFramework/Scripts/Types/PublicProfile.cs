using System;
using UnityEngine;
using System.Collections.Generic;
using ArabicSupport;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class PublicProfile
    {
        public string playerId;
        public string countryId;
        int _eloScore;
        public Sprite profilePicture;
        public string facebookUserId;
        public bool isOnline;
        public string creationDate;
        public string lastSeen;
        string _name;
        public DateTime lastSeenDateTime;
        public int totalGamesWon;
        public int totalGamesLost;
        public string avatarId;
        public string avatarBgColorId;
        public string creationDateShort;
        public bool isSubscriber;
        public string uploadedPicId;
        public int league;
        public Sprite leagueBorder;
        public int trophies2;

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = ArabicFixer.Fix(value, false, false);
            }
        }

        public bool isActive
        {
            get
            {
                DateTime activeTimeStamp = DateTime.UtcNow.AddDays(-1);
                return lastSeenDateTime.CompareTo(activeTimeStamp) >= 0;
            }
        }

        public int eloScore
        {
            get
            {
                if (CollectionsUtil.fakeEloScores.ContainsKey(playerId))
                {
                    CollectionsUtil.fakeEloScores.TryGetValue(playerId, out int fakeElo);
                    return fakeElo;
                }
                else
                {
                    return _eloScore;
                }
            }
            set
            {
                _eloScore = value;
            }
        }

    }
}

