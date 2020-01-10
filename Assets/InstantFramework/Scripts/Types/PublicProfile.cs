using System;
using UnityEngine;
using System.Collections.Generic;
using ArabicSupport;

namespace TurboLabz.InstantFramework
{
    public class PublicProfile
    {
        public string playerId;
        public string countryId;
        public int eloScore;
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

    }
}

