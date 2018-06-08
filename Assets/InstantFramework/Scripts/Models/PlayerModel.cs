/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;
using System;


// TODO: MOVE SAVE KEYS FOR FRAMEWORK!
using TurboLabz.InstantChess;

namespace TurboLabz.InstantFramework
{
    public class PlayerModel : IPlayerModel
    {
		[Inject] public ILocalDataService localDataService { get; set; }

        // Player Private Profile
        public string id { get; set; }
        public string tag { get; set; }
        public string name { get; set; }
        public string countryId { get; set; }
        public Sprite profilePicture { get; set; }
        public Sprite profilePictureBorder { get; set; }
        public Sprite profilePictureFB { get; set; }
        public int xp { get; set; }
        public int level { get; set; }
        public int nextMedalAt { get; set; }
        public int medals { get; set; }
        // public int totalGamesWon { get; }
        // public int totalGamesLost { get; }
        // public int totalGamesDrawn { get; }
        // public int totalGames { get; }

        // Player Public Profile
        // public PublicProfile publicProfile { get; }

        // Currency 
        public long currency1 { get; set; }
        public long bucks { get; set; }
        public long currency1Winnings { get; set; }

        // League & ELO
        public string leagueId { get; set; }
        public string league { get; set; }
        public string eloDivision { get; set; }
        public int eloScore { get; set; }
        public int eloTotalPlacementGames { get; set; }
        public int eloCompletedPlacementGames { get; set; }
        // public bool isEloEstablished { get; }

        // The keys of the dictionary are the IDs of the rooms.
        public IDictionary<string, RoomRecord> roomRecords { get; set; }

        // Social
        public bool isSocialNameSet { get; set; }
        // public bool hasExternalAuth { get; }
        public IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications { get; set; }

        // Ads Info
        public int adLifetimeImpressions { get; set; }
        public int adSlotImpressions { get; set; }                         // TODO: move to Ad Settings
        public long adSlotId { get; set; }                                 // TODO: move to Ad Settings

        // Inventory
        public string activeSkinId { get; set; }                           // TODO: move to prefs
        public string activeAvatarId { get; set; }                         // TODO: move to prefs
        public IOrderedDictionary<string, int> inventory { get; set; }

		public bool isEloEstablished
		{
			get
			{
				return (eloCompletedPlacementGames >= eloTotalPlacementGames);
			}
		}

		public bool hasExternalAuth
		{
			get
			{
				return (externalAuthentications.Count > 0);
			}
		}

		public int totalGamesWon
		{
			get
			{
				int count = 0;

				foreach (var roomRecord in roomRecords.Values)
				{
					count += roomRecord.gamesWon;
				}

				return count;
			}
		}

		public int totalGamesLost
		{
			get
			{
				int count = 0;

				foreach (var roomRecord in roomRecords.Values)
				{
					count += roomRecord.gamesLost;
				}

				return count;
			}
		}

		public int totalGamesDrawn
		{
			get
			{
				int count = 0;

				foreach (var roomRecord in roomRecords.Values)
				{
					count += roomRecord.gamesDrawn;
				}

				return count;
			}
		}

		public int totalGames
		{
			get
			{
				int count = 0;

				foreach (var roomRecord in roomRecords.Values)
				{
					count += roomRecord.gamesWon + roomRecord.gamesLost + roomRecord.gamesDrawn;
				}

				return count;
			}
		}

		public PublicProfile publicProfile
		{
			get
			{
				PublicProfile profile;
				profile.id = id;
				profile.name = name;
				profile.countryId = countryId;
				profile.level = level;
				profile.leagueId = leagueId;
				profile.roomRecords = roomRecords;
				profile.externalAuthentications = externalAuthentications;
				profile.profilePicture = profilePicture;
				profile.profilePictureBorder = profilePictureBorder;
				profile.eloDivision = eloDivision;
				profile.eloScore = eloScore;
				profile.eloTotalPlacementGames = eloTotalPlacementGames;
				profile.eloCompletedPlacementGames = eloCompletedPlacementGames;
				profile.league = league;
				profile.medals = medals;

				return profile;
			}
		}

		public void Load()
		{
			Reset();
			LoadFromFile();
		}

        public void Reset()
        {
            activeSkinId = null;
            adLifetimeImpressions = 0;
            adSlotImpressions = 0;
            adSlotId = 0;
			id = null;
			tag = null;
			name = null;
			countryId = null;
			profilePicture = null;
			profilePictureBorder = null;
			profilePictureFB = null;
			currency1 = 0;
			bucks = 0;
			currency1Winnings = 0;
			xp = 0;
			level = 0;
			leagueId = null;
			eloDivision = null;
			eloScore = 0;
			roomRecords = null;
			isSocialNameSet = false;
			externalAuthentications = null;
			league = null;
			nextMedalAt = 0;
			medals = 0;
        }

		public bool ownsVGood(string key)
		{
            return inventory.ContainsKey(key);
		}

		public void LoadFromFile()
		{
            /*
			if (!localDataService.FileExists(SaveKeys.PLAYER_SAVE_FILENAME))
			{
				return;
			}

			try
			{
				ILocalDataReader reader = localDataService.OpenReader(SaveKeys.PLAYER_SAVE_FILENAME);

				id = reader.Read<string>(SaveKeys.PLAYER_ID);
				activeSkinId = reader.Read<string>(SaveKeys.PLAYER_ACTIVE_SKIN_ID);
				//bucks = reader.Read<int>(SaveKeys.PLAYER_BUCKS);
				vGoods = reader.ReadList<string>(SaveKeys.PLAYER_VGOODS);
                adLifetimeImpressions = reader.Read<int>(SaveKeys.PLAYER_AD_LIFE_TIME_IMPRESSIONS);
                adSlotImpressions = reader.Read<int>(SaveKeys.PLAYER_AD_SLOT_IMPRESSIONS);
                adSlotId = reader.Read<long>(SaveKeys.PLAYER_AD_SLOT_ID);

				reader.Close();
			}
			catch (Exception e)
			{
				LogUtil.Log("Corrupt saved player data! " + e, "red");
				localDataService.DeleteFile(SaveKeys.PLAYER_SAVE_FILENAME);
				Reset();
			}
   */         
		}

		public void SaveToFile()
		{
            /*
			try
			{
				ILocalDataWriter writer = localDataService.OpenWriter(SaveKeys.PLAYER_SAVE_FILENAME);

				writer.Write<string>(SaveKeys.PLAYER_ID, id);
				writer.Write<string>(SaveKeys.PLAYER_ACTIVE_SKIN_ID, activeSkinId);
				//writer.Write<int>(SaveKeys.PLAYER_BUCKS, bucks);
				writer.WriteList<string>(SaveKeys.PLAYER_VGOODS, vGoods);
                writer.Write<int>(SaveKeys.PLAYER_AD_LIFE_TIME_IMPRESSIONS, adLifetimeImpressions);
                writer.Write<int>(SaveKeys.PLAYER_AD_SLOT_IMPRESSIONS, adSlotImpressions);
                writer.Write<long>(SaveKeys.PLAYER_AD_SLOT_ID, adSlotId);

				writer.Close();
			}
			catch (Exception e)
			{
				if (localDataService.FileExists(SaveKeys.PLAYER_SAVE_FILENAME))
				{
					localDataService.DeleteFile(SaveKeys.PLAYER_SAVE_FILENAME);
				}

				LogUtil.Log("Critical error when saving player data. File deleted. " + e, "red");
			}
   */         
		}

    }


	public struct PublicProfile
	{
		public string id;
		public string name;
		public string countryId;
		public int level;
		public string leagueId;
		public string eloDivision;
		public int eloScore;
		public int eloTotalPlacementGames;
		public int eloCompletedPlacementGames;
		public string league;
		public int medals;

		// The keys of the dictionary are the IDs of the rooms.
		public IDictionary<string, RoomRecord> roomRecords;

		public Sprite profilePicture;
		public Sprite profilePictureBorder;

		public IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications { get; set; }

		public bool hasExternalAuth
		{
			get
			{
				return (externalAuthentications.Count > 0);
			}
		}

		public bool isEloEstablished
		{
			get
			{
				return (eloCompletedPlacementGames >= eloTotalPlacementGames);
			}
		}
	}

	// The id field is also present in the RoomRecord since a room record must
	// always be able to refer to the room it belongs to from within itself.
	public struct RoomRecord
	{
		public string id;
		public int gamesWon;
		public int gamesLost;
		public int gamesDrawn;
		public int trophiesWon;
		public string roomTitleId;
	}

	public struct ExternalAuthData
	{
		public string id;
	}

}

