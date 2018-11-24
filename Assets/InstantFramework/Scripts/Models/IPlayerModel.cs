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


        // Ads Info
        int adLifetimeImpressions { get; set; }          

        // Inventory
        string activeSkinId { get; set; }   
        IOrderedDictionary<string, int> inventory { get; set; }


		// Friends
		Dictionary<string, Friend> friends { get; set; }
		Dictionary<string, Friend> blocked { get; set; }
        Dictionary<string, Friend> community { get; set; }
        bool busyRefreshingCommunity { get; set; }

        bool OwnsVGood(string key);
	}
}
