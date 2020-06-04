using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SocialEdge.Modules.PlayerModel;

namespace SocialEdge.Modules
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
        public PublicProfile publicProfile;


        public Friend Create(string playerId)
        {
            Friend friend = new Friend();
            friend.publicProfile = new PlayerModel().GetPublicProfile(playerId);
            friend.friendType = FRIEND_TYPE_SOCIAL;
            return friend;
        }


    }

}