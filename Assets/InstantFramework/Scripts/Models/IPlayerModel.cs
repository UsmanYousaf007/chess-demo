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
        string tag { get; set; } 
        string name { get; set; }
        string countryId { get; set; }
        int totalGamesWon { get; set; }
        int totalGamesLost { get; set; }
        int totalGamesDrawn { get; set; }
        long bucks { get; set; }
        int eloScore { get; set; }

        Sprite profilePic { get; set; }
        Sprite socialPic { get; set; }

        // Ads Info
        int adLifetimeImpressions { get; set; }          

        // Inventory
        string activeSkinId { get; set; }   
        IOrderedDictionary<string, int> inventory { get; set; }

        PublicProfile publicProfile { get; }

		// Friends
		IDictionary<string, Friend> friends { get; set; }
		IDictionary<string, Friend> blocked { get; set; }

        void Reset();
        bool OwnsVGood(string key);
	}
}
