/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using UnityEngine;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public interface IPlayerModel
    {
        string id { get; set; }
        long creationDate { get; set; }
        string tag { get; set; } 
        string name { get; set; }
        string countryId { get; set; }
        int totalGamesWon { get; set; }
        int totalGamesLost { get; set; }
        int totalGamesDrawn { get; set; }
        long bucks { get; set; }
        int eloScore { get; set; }
        bool isPremium { get; set; }
        string avatarId { get; set; }
        string avatarBgColorId { get; set; }
        int notificationCount { get; set; }
        string editedName { get; set; }
        bool newUser { get; set; }
        string skillLevel { get; set; }
        int playerFriendsCount { get; set; }
        bool isFBConnectRewardClaimed { get; set; }
        long subscriptionExipryTimeStamp { get; set; }

        // Ads Info
        int adLifetimeImpressions { get; set; }
        long removeAdsTimeStamp { get; set; }
        int removeAdsTimePeriod { get; set; }
        int cpuPowerupUsedCount { get; set; }

        // Inventory
        string activeSkinId { get; set; }   
        IOrderedDictionary<string, int> inventory { get; set; }


		// Friends
		Dictionary<string, Friend> friends { get; set; }
		Dictionary<string, Friend> blocked { get; set; }
        Dictionary<string, Friend> community { get; set; }
        Dictionary<string, Friend> search { get; set; }
        bool busyRefreshingCommunity { get; set; }

        bool OwnsVGood(string key);

        int PowerUpHintCount { get; }
        int PowerUpHindsightCount { get; }
        int PowerUpSafeMoveCount { get; }

        bool HasRemoveAds(IAdsSettingsModel adsSettingsModel);
        bool HasAdsFreePeriod(IAdsSettingsModel adsSettingsModel);
        PlayerInventoryVO GetPlayerInventory();

        Friend GetFriend(string friendId);
        bool IsFriend(string friendId);
        bool HasSubscription();
    }
}
