using System;
using UnityEngine;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
	public class Friend
	{
        public const string FRIEND_TYPE_SOCIAL = "social";
        public const string FRIEND_TYPE_COMMUNITY = "community";
        public const string FRIEND_TYPE_FAVOURITE = "favourite";

        public string playerId;
        public string friendType;
		public int gamesWon;
		public int gamesLost;
		public int gamesDrawn;
        public long lastMatchTimestamp;
        public bool removedFromRecentPlayed;
        public PublicProfile publicProfile;
	}
}

