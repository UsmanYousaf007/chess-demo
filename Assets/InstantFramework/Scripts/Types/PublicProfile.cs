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
        string _name;

    }
}

