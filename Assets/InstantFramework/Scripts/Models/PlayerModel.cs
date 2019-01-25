/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class PlayerModel : IPlayerModel
    {
        public string id { get; set; }
        public long creationDate { get; set; }
        public string tag { get; set; } 
        public string name { get; set; }
        public string countryId { get; set; }
        public int totalGamesWon { get; set; }
        public int totalGamesLost { get; set; }
        public int totalGamesDrawn { get; set; }
        public long bucks { get; set; }
        public int eloScore { get; set; }

        // Ads Info
        public int adLifetimeImpressions { get; set; }
        public long removeAdsTimeStamp { get; set; }

        // Inventory
        public string activeSkinId { get; set; } = null;    
        public IOrderedDictionary<string, int> inventory { get; set; }

		// Friends
		public Dictionary<string, Friend> friends { get; set; }
		public Dictionary<string, Friend> blocked { get; set; }
        public Dictionary<string, Friend> community { get; set; }
        public bool busyRefreshingCommunity { get; set; }

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


            // Ads Info
            adLifetimeImpressions = 0;
            removeAdsTimeStamp = 0;

            // Inventory
            inventory = new OrderedDictionary<string, int>();

			// Friends
			friends = new Dictionary<string, Friend>();
			blocked = new Dictionary<string, Friend>();
            community = new Dictionary<string, Friend>();
            busyRefreshingCommunity = false;
        }

		public bool OwnsVGood(string key)
		{
            return inventory.ContainsKey(key);
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

        public bool hasRemoveAds(IAdsSettingsModel adsSettingsModel)
        {
            return OwnsVGood(GSBackendKeys.SHOP_ITEM_FEATURE_REMOVE_ADS_PERM) ||
                    (TimeUtil.TimeToExpireString(creationDate, adsSettingsModel.freeNoAdsPeriod) != null) ||
                    (OwnsVGood(GSBackendKeys.SHOP_ITEM_FEATURE_REMOVE_ADS_30) && (TimeUtil.TimeToExpireString(removeAdsTimeStamp, 30) != null));
        }

        public bool hasRemoveAdsFreePeriod(IAdsSettingsModel adsSettingsModel)
        {
            if (OwnsVGood(GSBackendKeys.SHOP_ITEM_FEATURE_REMOVE_ADS_PERM))
            {
                return false;
            }

            if (OwnsVGood(GSBackendKeys.SHOP_ITEM_FEATURE_REMOVE_ADS_30))
            {
                if (TimeUtil.TimeToExpireString(removeAdsTimeStamp, 30) != null)
                {
                    return false;
                }
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
    }
}

