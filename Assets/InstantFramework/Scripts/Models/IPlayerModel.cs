/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:36:26 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public interface IPlayerModel
    {
        string activeSkinId { get; set; }			// TODO: move to prefs
		string activeAvatarsId { get; set; }		// TODO: move to prefs

		List<string> vGoods { get; set; }

		bool ownsVGood(string key);

		void Reset();
		void SaveToFile();

		string id { get; set; }
		string tag { get; set; }
		string name { get; set; }
		string countryId { get; set; }
		Sprite profilePicture { get; set; }
		Sprite profilePictureBorder { get; set; }
		Sprite profilePictureFB { get; set; }
		long currency1 { get; set; }
		long currency2 { get; set; }
		long currency1Winnings { get; set; }
		int xp { get; set; }
		int level { get; set; }
		string leagueId { get; set; }
		string eloDivision { get; set; }
		int eloScore { get; set; }
		int eloTotalPlacementGames { get; set; }
		int eloCompletedPlacementGames { get; set; }
		string league { get; set; }
		int nextMedalAt { get; set; }
		int medals { get; set; }
		bool isEloEstablished { get; }

		// The keys of the dictionary are the IDs of the rooms.
		IDictionary<string, RoomRecord> roomRecords { get; set; }

		bool isSocialNameSet { get; set; }
		IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications { get; set; }
		bool hasExternalAuth { get; }
		int totalGamesWon { get; }
		int totalGamesLost { get; }
		int totalGamesDrawn { get; }
		int totalGames { get; }
		PublicProfile publicProfile { get; }

		int adLifetimeImpressions { get; set; }
		int adSlotImpressions { get; set; }
		long adSlotId { get; set; }
	}
}
