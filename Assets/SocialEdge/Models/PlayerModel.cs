using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SocialEdge.Modules
{
    public class PlayerModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public PrivateProfile privateProfile { get; set; }
        public PublicProfile publicProfile { get; set; }



        public PlayerModel()
        {
            privateProfile = new PrivateProfile();
            privateProfile.friends = new Dictionary<string, Friend>();
            privateProfile.community = new Dictionary<string, Friend>();
            privateProfile.blocked = new Dictionary<string, string>();

            publicProfile = new PublicProfile();
            publicProfile.playerActiveInventory = new List<Inventory>();
            publicProfile.activeChallenges = new List<Match>();
            publicProfile.pendingChallenges = new List<Match>();
            publicProfile.adsRewardData = new List<Ads>();

        }

        public PlayerModel Get(string id)
        {
            PlayerModel player = new PlayerModel();
            player.privateProfile = GetPrivateProfile(id);
            player.publicProfile = GetPublicProfile(id);
            return player;
        }

        public PrivateProfile GetPrivateProfile(string id)
        {
            return new PrivateProfile();
        }

        public PublicProfile GetPublicProfile(string id)
        {
            return new PublicProfile();
        }

        #region Player Profiles
        public class PrivateProfile
        {
            public string tag;
            public int eloCompletedPlacementGames;
            public int eloScore;
            public int gamesWon;
            public int gamesLost;
            public int gamesDrawn;
            public string countryFlag;
            public Dictionary<string, Friend> friends;
            public Dictionary<string, Friend> community;
            public Dictionary<string, string> blocked;
            public PrivateProfile()
            {
                countryFlag = null;
            }
        }

        public class PublicProfile
        {

            public int adLifetimeImpressions;
            public List<Inventory> playerActiveInventory;
            public bool isInitialized;
            public bool isBot;
            public int botDifficulty;
            public string currentChallengeId;

            public List<Match> activeChallenges;
            public List<Match> pendingChallenges;
            public bool firstLongMatchCompleted;
            public int removeAdsTimeStamp;
            public int removeAdsTimePeriod;
            public bool isPremium;
            public bool isSearchRegistered;
            public string clientVersion;//: '1.0.7',
            public string editedName;
            public bool isFBConnectRewardClaimed;
            public int cpuPowerupUsedCount;
            public int totalGamesCount;
            public int totalPowerupUsageCount;
            public int eventTimeStamp;
            public int subscriptionExpiryTime;
            public List<Ads> adsRewardData;


            #endregion
            public PublicProfile()
            {
                clientVersion = "1.0.7";
                currentChallengeId = null;
            }
        }
    }

    public class Ads
    {
    }

    public class Match
    {
    }

    public class Inventory
    {
    }
}