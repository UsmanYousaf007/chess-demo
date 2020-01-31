/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;
using ArabicSupport;

namespace TurboLabz.InstantFramework
{
    public class PlayerModel : IPlayerModel
    {
        [Inject] public IBackendService backendService { get; set; }

        public string id { get; set; }
        public long creationDate { get; set; }
        public string tag { get; set; } 
        public string countryId { get; set; }
        public int totalGamesWon { get; set; }
        public int totalGamesLost { get; set; }
        public int totalGamesDrawn { get; set; }
        public long bucks { get; set; }
        public int eloScore { get; set; }
        public bool isPremium { get; set; }
        public string avatarId { get; set; }
        public string avatarBgColorId { get; set; }
        public int notificationCount { get; set; }
        public string editedName { get; set; }
        public bool newUser { get; set; }
        public string skillLevel { get; set; }
        public int playerFriendsCount { get; set; }
        public bool isFBConnectRewardClaimed { get; set; }
        public int cpuPowerupUsedCount { get; set; }
        public long subscriptionExipryTimeStamp { get; set; }
        public string renewDate { get; set; }

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

        // Ads Info
        public int adLifetimeImpressions { get; set; }
        public long removeAdsTimeStamp { get; set; }
        public int removeAdsTimePeriod { get; set; }

        // Inventory
        public string activeSkinId { get; set; } = null;    
        public IOrderedDictionary<string, int> inventory { get; set; }

		// Friends
		public Dictionary<string, Friend> friends { get; set; }
		public Dictionary<string, Friend> blocked { get; set; }
        public Dictionary<string, Friend> community { get; set; }
        public Dictionary<string, Friend> search { get; set; }
        public bool busyRefreshingCommunity { get; set; }

        // Ads Reward Data
        public int rewardIndex { get; set; }
        public string rewardShortCode { get; set; }
        public int rewardQuantity { get; set; }
        public float rewardCurrentPoints { get; set; }
        public float rewardPointsRequired { get; set; }

        private bool isSubscriber = false;

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            id = null;
            creationDate = 0;
            tag = null;
            name = null;
            countryId = null;
            totalGamesWon = 0;
            totalGamesLost = 0;
            totalGamesDrawn = 0;
            bucks = 0;
            eloScore = 0;
            isPremium = false;
            avatarId = null;
            avatarBgColorId = null;
            notificationCount = 0;
            editedName = "";
            subscriptionExipryTimeStamp = 0;
            renewDate = "";

            // Ads Info
            adLifetimeImpressions = 0;
            removeAdsTimeStamp = 0;
            removeAdsTimePeriod = 0;
            playerFriendsCount = 0;
            isFBConnectRewardClaimed = false;
            cpuPowerupUsedCount = 0;

            // Inventory
            inventory = new OrderedDictionary<string, int>();

			// Friends
			friends = new Dictionary<string, Friend>();
			blocked = new Dictionary<string, Friend>();
            community = new Dictionary<string, Friend>();
            search = new Dictionary<string, Friend>();

            busyRefreshingCommunity = false;

            rewardIndex = 1; // by default skins at index 0 will be unlocked
            rewardCurrentPoints = 0;
            rewardPointsRequired = 0;
            rewardShortCode = "";
            rewardQuantity = 0;
        }

		public bool OwnsVGood(string key)
		{
			TLUtils.LogUtil.LogNullValidation(key, "key");
		
            return key != null && inventory.ContainsKey(key);
		}

        public int PowerUpHintCount
        {
            get
            {
                return OwnsVGood(GSBackendKeys.PowerUp.HINT) ? inventory[GSBackendKeys.PowerUp.HINT] : 0;
            }
        }

        public int PowerUpHindsightCount
        {
            get
            {
                return OwnsVGood(GSBackendKeys.PowerUp.HINDSIGHT) ? inventory[GSBackendKeys.PowerUp.HINDSIGHT] : 0;
            }
        }

        public int PowerUpSafeMoveCount
        {
            get
            {
                return OwnsVGood(GSBackendKeys.PowerUp.SAFE_MOVE) ? inventory[GSBackendKeys.PowerUp.SAFE_MOVE] : 0;
            }
        }

        public bool HasRemoveAds(IAdsSettingsModel adsSettingsModel)
        {
            //return OwnsVGood(GSBackendKeys.SHOP_ITEM_FEATURE_REMOVE_ADS_PERM) ||
            //        (TimeUtil.TimeToExpireString(creationDate, adsSettingsModel.freeNoAdsPeriod) != null) ||
            //        (TimeUtil.TimeToExpireString(removeAdsTimeStamp, removeAdsTimePeriod) != null);

            return HasSubscription();
        }

        public bool HasSubscription()
        {
            if (!isSubscriber)
            {
                isSubscriber = subscriptionExipryTimeStamp > backendService.serverClock.currentTimestamp;
            }

            return isPremium || isSubscriber;
        }

        public bool HasAdsFreePeriod(IAdsSettingsModel adsSettingsModel)
        {
            if (OwnsVGood(GSBackendKeys.SHOP_ITEM_FEATURE_REMOVE_ADS_PERM))
            {
                return false;
            }

            if (TimeUtil.TimeToExpireString(removeAdsTimeStamp, removeAdsTimePeriod) != null)
            {
                return false;
            }

            if (TimeUtil.TimeToExpireString(creationDate, adsSettingsModel.freeNoAdsPeriod) != null)
            {
                return true;
            }

            return false;
        }

        public PlayerInventoryVO GetPlayerInventory()
        {
            PlayerInventoryVO playerInventoryVO = new PlayerInventoryVO();
            playerInventoryVO.coinCount = bucks;
            playerInventoryVO.hintCount = PowerUpHintCount;
            playerInventoryVO.safeMoveCount = PowerUpSafeMoveCount;
            playerInventoryVO.hindsightCount = PowerUpHindsightCount;

            return playerInventoryVO;
        }

        public Friend GetFriend(string friendId)
        {
            Friend friend = null;
            if (friends.ContainsKey(friendId))
            {
                friend = friends[friendId];
            }
            else if (community.ContainsKey(friendId))
            {
                friend = community[friendId];
            }
            else if (search.ContainsKey(friendId))
            {
                friend = search[friendId];
            }

            return friend;
        }

        public bool IsFriend(string friendId)
        {
            return friends.ContainsKey(friendId);
        }

        public void UpdateGoodsInventory(string key, int quantity)
        {
            if (inventory.ContainsKey(key))
            {
                inventory[key] += quantity;
            }
            else
            {
                inventory.Add(key, quantity);
            }
        }

        public AdsRewardVO GetAdsRewardsData()
        {
            var adsRewardVO = new AdsRewardVO();
            adsRewardVO.rewardIndex = rewardIndex;
            adsRewardVO.shortCode = rewardShortCode;
            adsRewardVO.quantity = rewardQuantity;
            adsRewardVO.currentPoints = rewardCurrentPoints;
            adsRewardVO.requiredPoints = rewardPointsRequired;
            return adsRewardVO;
        }

    }
}

