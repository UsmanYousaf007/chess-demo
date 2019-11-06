using System;
using UnityEngine;
using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class PicsModel : IPicsModel
    {
        // Services
        [Inject] public ILocalDataService localDataService { get; set; }

        private const string PIC_KEY = "pic";
        private const string PIC_FILE_PREFIX = "fp";
        private Dictionary<string, Sprite> memCache = new Dictionary<string, Sprite>();

        public void SetPlayerPic(string playerId, Sprite sprite, bool saveOnDisk = true)
        {
            if (sprite == null)
                return;

            string filename = PIC_FILE_PREFIX + playerId;

            try
            {
                if (saveOnDisk)
                {
                    ILocalDataWriter writer = localDataService.OpenWriter(filename);
                    writer.Write(PIC_KEY, sprite);
                    writer.Close();
                }
                
                TLUtils.LogUtil.LogNullValidation(playerId, "playerId");

                if (playerId != null)
                {

                    if (memCache.ContainsKey(playerId))
                    {
                        memCache[playerId] = sprite;
                    }
                    else
                    {
                        memCache.Add(playerId, sprite);
                    }
                }

            }
            catch (Exception e)
            {
                // something went wrong, get rid of the file
                localDataService.DeleteFile(filename);
                Debug.Log(e.ToString());
            }
        }

        public Sprite GetPlayerPic(string playerId)
        {
            LogUtil.LogNullValidation(playerId, "playerId");

            if(playerId  == null)
            {
                return null;
            }

            if (memCache.ContainsKey(playerId))
            {
                return memCache[playerId];
            }

            string filename = PIC_FILE_PREFIX + playerId;

            try
            {
                if (localDataService.FileExists(filename))
                {
                    ILocalDataReader reader = localDataService.OpenReader(filename);
    
                    Sprite pic = null;

                    if (reader.HasKey(PIC_KEY))
                    {
                        pic = reader.Read<Sprite>(PIC_KEY);
                    }

                    reader.Close();
                    memCache.Add(playerId, pic);
                    return pic;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return null;
            }

            return null;
        }

        public void SetFriendPics(Dictionary<string, Friend> friends)
        {
            foreach (KeyValuePair<string, Friend> entry in friends)
            {
                SetPlayerPic(entry.Key, entry.Value.publicProfile.profilePicture);
            }
        }

        public Dictionary<string, Sprite> GetFriendPics(List<string> playerIds)
        {
            Dictionary<string, Sprite> pics = new Dictionary<string, Sprite>();

            foreach (string playerId in playerIds)
            {
                Sprite pic = GetPlayerPic(playerId);
                if (pic != null) pics.Add(playerId, pic);
            }

            return pics;
        }

        public void DeleteFriendPic(string playerId)
        {
            string filename = PIC_FILE_PREFIX + playerId;

            try
            {
                localDataService.DeleteFile(filename);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
}

