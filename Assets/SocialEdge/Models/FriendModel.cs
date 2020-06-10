using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SocialEdge.Modules.PlayerModel;

namespace SocialEdge.Modules
{
    public class FriendModel
    {
        public string playerId;
        public string friendType;
        public int gamesWon;
        public int gamesLost;
        public int gamesDrawn;
        public long lastMatchTimestamp;
        public PublicProfile publicProfile;
    }

}