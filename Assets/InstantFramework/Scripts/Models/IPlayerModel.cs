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
        // Player Private Profile
		string id { get; set; }
		string tag { get; set; }
		string name { get; set; }
		string countryId { get; set; }
		Sprite profilePicture { get; set; }
		Sprite profilePictureBorder { get; set; }
		Sprite profilePictureFB { get; set; }
        int totalGamesWon { get; set; }
        int totalGamesLost { get; set; }
        int totalGamesDrawn { get; set; }
        int totalGamesPlayed { get; set; }
        int totalGamesAbandoned { get; set; }

        // Player Public Profile
        PublicProfile publicProfile { get; }

        // Currency 
		long bucks { get; set; }

        // Rating
		int eloScore { get; set; }

        // Social
		bool isSocialNameSet { get; set; }
		bool hasExternalAuth { get; }
        IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications { get; set; }

        // Ads Info
		int adLifetimeImpressions { get; set; }
		int adSlotImpressions { get; set; }
		long adSlotId { get; set; }

        // Inventory
        string activeSkinId { get; set; } 
        string activeAvatarId { get; set; } 
        IOrderedDictionary<string, int> inventory { get; set; }

        void Reset();
        void SaveToFile();
        bool ownsVGood(string key);
	}
}
