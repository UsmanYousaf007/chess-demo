using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
namespace SocialEdge.Models
{
    public class SettingsView
    {
        public GameSettings gameSettings;
        public GameEconomySettings gameEconomySettings;
    }

    public class GameSettings
    {

        [JsonProperty("Meta")]
        MetaSettings Meta { get; set; }
        [JsonProperty("Community")]
        CommunitySettings Community { get; set; }
        [JsonProperty("Friends")]
        FriendsSettings Friends { get; set; }
        [JsonProperty("Common")]
        CommonSettings Common { get; set; }

    }

    public class MetaSettings
    {
        public string androidURL;
        public string iosURL;
        public int rateAppThreshold;
        public int backendAppVersion;
        public string contactSupportURL;
    }

    public class CommunitySettings
    {
        public int expireAfterSeconds;
        public int maxSuggests;
        public int minSuggests;
        public int domesticPct;
    }

    public class FriendsSettings
    {

        public int searchExpireAfterSeconds;
        public int searchPageMax;
    }

    public class CommonSettings
    {
        public bool maintenanceFlag;
        public bool maintenanceWarningFlag;
        public string nmaintenanceWarningMessage;
        public string maintenanceMessage;
        public string updateMessage;
        public int maxLongMatchCount;
        public int maxFriendsCount;
        public int maxCommunityMatches;
        public int maxRecentlyCompletedMatchCount;
        public string maintenanceWarningBgColor;

        [JsonProperty("ios")]
        public MobileVersionSettings ios;
        [JsonProperty("android")]
        public MobileVersionSettings android;
        [JsonProperty("premium")]
        public PremiumSettings premium;
    }

    public class MobileVersionSettings
    {
        public string minimumClientVersion;
        public string gameUpdateBannerMsg;
        public string manageSubscriptionURL;
    }

    public class PremiumSettings
    {
        public int maxLongMatchCount;
        public int maxFriendsCount;
    }

    public class GameEconomySettings
    {
        [JsonProperty("PlayerDefaults")]
        PlayerDefaultsSettings playerDefaults;
        [JsonProperty("PlayerDefaultOwnedItems")]
        List<PlayerDefaultOwnedItemsSettings> playerDefaultOwnedItems;
        [JsonProperty("Rewards")]
        RewardsSettings rewards;
        [JsonProperty("Ads")]
        AdsSettings ads;
    }

    public class PlayerDefaultsSettings
    {
        public int CURRENCY2;
    }

    public class PlayerDefaultOwnedItemsSettings
    {
        public string shopItemKey;
        public int quantity;
    }

    public class RewardsSettings
    {
        public decimal matchWinReward;
        public decimal matchWinAdReward;
        public decimal matchRunnerUpReward;
        public decimal matchRunnerUpAdReward;
        public decimal rewardMatchPromotional;
        public int failSafe;
        public decimal facebookConnectReward;
    }

    public class AdsSettings
    {
        public int adsGlobalCap;
        public int adsInterstitialCap;
        public int adsRewardedVideoCap;
        public int resignCap;
        public int ADS_SLOT_HOUR;
        public int ADS_FREE_NO_ADS_PERIOD;
        public int minutesForVictoryInteralAd;
        public int autoSubscriptionDlgThreshold;
        public int daysPerAutoSubscriptionDlgThreshold;
        public int sessionsBeforePregameAd;
        public int maxPregameAdsPerDay;
        public int intervalsBetweenPregameAds;
        public decimal waitForPregameAdLoadSeconds;
        public bool showPregameOneMinute;
    }
}