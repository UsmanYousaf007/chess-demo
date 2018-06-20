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
        public string tag { get; set; } 
        public string name { get; set; }
        public string countryId { get; set; }
        public Sprite profilePicture { get; set; }
        public int totalGamesWon { get; set; }
        public int totalGamesLost { get; set; }
        public int totalGamesDrawn { get; set; }
        public long bucks { get; set; }
        public int eloScore { get; set; }
        public IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications { get; set; }

        // Ads Info
        public int adLifetimeImpressions { get; set; }
        public int adSlotImpressions { get; set; }    
        public long adSlotId { get; set; }            

        // Inventory
        public string activeSkinId { get; set; }      
        public string activeAvatarId { get; set; }    
        public IOrderedDictionary<string, int> inventory { get; set; }

		public PublicProfile publicProfile
		{
			get
			{
                PublicProfile profile = new PublicProfile();
				profile.id = id;
				profile.name = name;
				profile.countryId = countryId;
				profile.profilePicture = profilePicture;
				profile.eloScore = eloScore;

				return profile;
			}
		}

        public void Reset()
        {
            id = null;
            tag = null;
            name = null;
            countryId = null;
            Sprite profilePicture = null;
            totalGamesWon = 0;
            totalGamesLost = 0;
            totalGamesDrawn = 0;
            bucks = 0;
            eloScore = 0;
            IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications = 
                new Dictionary<ExternalAuthType, ExternalAuthData>();

            // Ads Info
            adLifetimeImpressions = 0;
            adSlotImpressions = 0;
            adSlotId  = 0;

            // Inventory
            activeSkinId = null;
            activeAvatarId = null;
            IOrderedDictionary<string, int> inventory =
                new OrderedDictionary<string, int>();
        }

		public bool OwnsVGood(string key)
		{
            return inventory.ContainsKey(key);
		}
    }
}

