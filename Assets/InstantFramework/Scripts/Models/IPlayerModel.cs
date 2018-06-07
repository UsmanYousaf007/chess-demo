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
        int xp { get; set; }
        int level { get; set; }
        int nextMedalAt { get; set; }
        int medals { get; set; }
        int totalGamesWon { get; }
        int totalGamesLost { get; }
        int totalGamesDrawn { get; }
        int totalGames { get; }

        // Player Public Profile
        PublicProfile publicProfile { get; }

        // Currency 
		long currency1 { get; set; }
		long bucks { get; set; }
		long currency1Winnings { get; set; }

        // League & Rating
		string leagueId { get; set; }
        string league { get; set; }
		string eloDivision { get; set; }
		int eloScore { get; set; }
		int eloTotalPlacementGames { get; set; }
		int eloCompletedPlacementGames { get; set; }
		bool isEloEstablished { get; }

		// The keys of the dictionary are the IDs of the rooms.
		IDictionary<string, RoomRecord> roomRecords { get; set; }

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
