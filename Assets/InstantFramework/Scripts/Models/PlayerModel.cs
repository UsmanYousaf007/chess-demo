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
using TurboLabz.InstantGame;

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
        public int totalGamesWon { get; set; }
        public int totalGamesLost { get; set; }
        public int totalGamesDrawn { get; set; }
        public int totalGamesAbandoned { get; set; }
        public int totalGamesPlayed { get; set; }

        // Player Public Profile
        // public PublicProfile publicProfile { get; }

        // Currency 
        public long bucks { get; set; }

        // League & ELO
        public int eloScore { get; set; }

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

		public bool hasExternalAuth
		{
			get
			{
				return (externalAuthentications.Count > 0);
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
				profile.externalAuthentications = externalAuthentications;
				profile.profilePicture = profilePicture;
				profile.profilePictureBorder = profilePictureBorder;
				profile.eloScore = eloScore;

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
			bucks = 0;
			eloScore = 0;
			isSocialNameSet = false;
			externalAuthentications = null;
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
		public int eloScore;
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
	}

	public struct ExternalAuthData
	{
		public string id;
	}

}

