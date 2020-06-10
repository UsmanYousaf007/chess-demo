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
            privateProfile.friends = new Dictionary<string, FriendModel>();
            privateProfile.community = new Dictionary<string, FriendModel>();
            privateProfile.blocked = new Dictionary<string, string>();
            privateProfile.playerActiveInventory = new List<Inventory>();
            privateProfile.activeChallenges = new List<Match>();
            privateProfile.pendingChallenges = new List<Match>();
            privateProfile.adsRewardData = new List<Ads>();

            publicProfile = new PublicProfile();

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
        public class PublicProfile
        {     
            public string tag;
            public int eloCompletedPlacementGames;
            public int eloScore;
            public int gamesWon;
            public int gamesLost;
            public int gamesDrawn;
            public string countryId;

            /*In old code, these were private members of profile but converted to public or were gamesparks attributes*/
            public bool isOnline;
            public bool isSubscriber;
            public string name;
            public List<string> externalIds;
            public DateTime creationDate;
            public List<Inventory> activeInventory;
            public PublicProfile()
            {
                countryId = null;
            }
        }

        public class PrivateProfile
        {

            public int adLifetimeImpressions;
            public List<Inventory> playerActiveInventory;
            public bool isInitialized;
            public bool isBot;
            public int botDifficulty;
            public string currentChallengeId;
            public Dictionary<string, FriendModel> friends;
            public Dictionary<string, FriendModel> community;
            public Dictionary<string, string> blocked;
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
            public int eloScore;
            public List<Ads> adsRewardData;


            #endregion
            public PrivateProfile()
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

    //    var getPublicDisplayName = function(sparkPlayer) {
    //        var fullName = sparkPlayer.getDisplayName();
    //    var nameSplit = fullName.split(" ");
    //    var first = "";
    //    var last = "";
    //    var firstNameLastInitial = fullName;
    //        if (nameSplit.length > 1)
    //        {
    //            for (var i = 0; i<nameSplit.length - 1; i++)
    //            {
    //                first = first + nameSplit[i];
    //            }
    //last = nameSplit[nameSplit.length - 1];
    //            var lastInitial = last[0];
    //firstNameLastInitial = first + " " + lastInitial + ".";
    //        }

    //        return firstNameLastInitial;
    //    }

    //public string GetPublicDisplayName()
    //{
    //    var fullName = this.GetPublicDisplayName();
    //}
}