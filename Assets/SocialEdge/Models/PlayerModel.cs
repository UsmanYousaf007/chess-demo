using System;
using System.Collections.Generic;


public class PlayerModel
{
    public PrivateProfile privateProfile { get; set; }
    public PublicProfile publicProfile { get; set; }



    public PlayerModel()
    {
        privateProfile = new PrivateProfile();
        publicProfile = new PublicProfile();
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
        public List<PlayerModel> friends;
        public List<PlayerModel> community;
        public List<PlayerModel> blocked;
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
