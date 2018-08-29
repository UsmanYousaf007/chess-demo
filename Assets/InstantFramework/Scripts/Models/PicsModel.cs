using System;
using UnityEngine;
using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public struct PicResult
    {
        public Sprite pic;
        public bool onlineRefreshRequired;
    }

    public class PicsModel : IPicsModel
    {
        // Services
        [Inject] public ILocalDataService localDataService { get; set; }

        private const string PLAYER_PIC_FILE = "playerPicFile";
        private const string PLAYER_PIC_KEY = "playerPicKey";
        private const string FRIENDS_PICS_FILE = "friendsPicsFile";

        public void SetPlayerPic(string playerId, Sprite sprite)
        {
            try
            {
                ILocalDataWriter writer = localDataService.OpenWriter(PLAYER_PIC_FILE);
                writer.Write(PLAYER_PIC_KEY, sprite);
                writer.Close();

                LogUtil.Log("Wrote pic for: " + playerId, "cyan");
            }
            catch (Exception e)
            {
                // something went wrong, get rid of the file
                localDataService.DeleteFile(PLAYER_PIC_FILE);
                Debug.Log(e.ToString());
            }
        }

        public Sprite GetPlayerPic(string playerId)
        {
            try
            {
                if (localDataService.FileExists(PLAYER_PIC_FILE))
                {
                    ILocalDataReader reader = localDataService.OpenReader(PLAYER_PIC_FILE);
                    string key = PLAYER_PIC_KEY;
                    Sprite pic = null;

                    if (reader.HasKey(key))
                    {
                        pic = reader.Read<Sprite>(key);
                        LogUtil.Log("Got pic for: " + playerId, "cyan");
                    }

                    reader.Close();
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
            try
            {
                if (localDataService.FileExists(FRIENDS_PICS_FILE))
                {
                    localDataService.DeleteFile(FRIENDS_PICS_FILE);
                }

                ILocalDataWriter writer = localDataService.OpenWriter(FRIENDS_PICS_FILE);

                foreach (KeyValuePair<string, Friend> entry in friends)
                {
                    writer.Write(entry.Key, entry.Value.publicProfile.profilePicture);
                    LogUtil.Log("Wrote pic for: " + entry.Key, "cyan");
                }

                writer.Close();
            }
            catch (Exception e)
            {
                // something went wrong, get rid of the file
                localDataService.DeleteFile(FRIENDS_PICS_FILE);
                Debug.Log(e.ToString());
            }
        }

        public Dictionary<string, Sprite> GetFriendPics(List<string> playerIds)
        {
            try
            {
                if (localDataService.FileExists(FRIENDS_PICS_FILE))
                {
                    Dictionary<string, Sprite> pics = new Dictionary<string, Sprite>();
                    ILocalDataReader reader = localDataService.OpenReader(FRIENDS_PICS_FILE);

                    foreach (string playerId in playerIds)
                    {
                        if (reader.HasKey(playerId))
                        {
                            pics.Add(playerId, reader.Read<Sprite>(playerId));
                            LogUtil.Log("Got pic for: " + playerId, "cyan");
                        }
                    }
                   
                    reader.Close();
                    return pics;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return null;
            }

            return null;
        }
    }
}

