/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:37:03 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System.Collections.Generic;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;
using System;

namespace TurboLabz.InstantChess
{
    public class PlayerModel : IPlayerModel
    {
		[Inject] public ILocalDataService localDataService { get; set; }

        public string id { get; set; }
        public string activeSkinId { get; set; }
		public int bucks { get; set; }
		public List<string> vGoods { get; set; }
        public int totalAdImpressions { get; set; }
        public int slotAdImpressions { get; set; }
        public double slotStartTime { get; set; }

		[PostConstruct]
		public void Load()
		{
			Reset();
			LoadFromFile();
		}

        public void Reset()
        {
            id = CPUSettings.DEFAULT_PLAYER_ID;
			bucks = CPUSettings.DEFAULT_STARTING_BUCKS;
			vGoods = new List<string>();
			for (int i = 0; i < CPUSettings.DEFAULT_VGOODS.Length; i++) 
			{
				vGoods.Add(CPUSettings.DEFAULT_VGOODS[i]);
			}
			activeSkinId = CPUSettings.DEFAULT_VGOODS[0];
            totalAdImpressions = 0;
            slotAdImpressions = 0;
            slotStartTime = 0;
        }

		public bool ownsVGood(string key)
		{
			bool found = false;
			int i = 0;
			while (!found && i < vGoods.Count) 
			{
				found = vGoods[i] == key;
				if (!found) 
				{
					i++;
				}
			}

			return found;
		}

		public void LoadFromFile()
		{
			if (!localDataService.FileExists(SaveKeys.PLAYER_SAVE_FILENAME))
			{
				return;
			}

			try
			{
				ILocalDataReader reader = localDataService.OpenReader(SaveKeys.PLAYER_SAVE_FILENAME);

				id = reader.Read<string>(SaveKeys.PLAYER_ID);
				activeSkinId = reader.Read<string>(SaveKeys.PLAYER_ACTIVE_SKIN_ID);
				bucks = reader.Read<int>(SaveKeys.PLAYER_BUCKS);
				vGoods = reader.ReadList<string>(SaveKeys.PLAYER_VGOODS);

				reader.Close();
			}
			catch (Exception e)
			{
				LogUtil.Log("Corrupt saved player data! " + e, "red");
				localDataService.DeleteFile(SaveKeys.PLAYER_SAVE_FILENAME);
				Reset();
			}
		}

		public void SaveToFile()
		{
			try
			{
				ILocalDataWriter writer = localDataService.OpenWriter(SaveKeys.PLAYER_SAVE_FILENAME);

				writer.Write<string>(SaveKeys.PLAYER_ID, id);
				writer.Write<string>(SaveKeys.PLAYER_ACTIVE_SKIN_ID, activeSkinId);
				writer.Write<int>(SaveKeys.PLAYER_BUCKS, bucks);
				writer.WriteList<string>(SaveKeys.PLAYER_VGOODS, vGoods);

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
		}

    }
}
