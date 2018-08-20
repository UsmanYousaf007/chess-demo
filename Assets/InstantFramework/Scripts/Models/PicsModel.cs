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

        private const string LOCAL_FACEBOOK_DATA_FILE = "localFacebookDataFile";
        private const string LOCAL_FACEBOOK_DATA_PIC = "localFacebookDataPic";

        public void SetPic(string playerId, Sprite sprite)
        {
            try
            {
                ILocalDataWriter writer = localDataService.OpenWriter(LOCAL_FACEBOOK_DATA_FILE);
                writer.Write(LOCAL_FACEBOOK_DATA_PIC + playerId, sprite);
                writer.Close();

                LogUtil.Log("Wrote pic for: " + playerId, "cyan");
            }
            catch (Exception e)
            {
                // something went wrong, get rid of the file
                localDataService.DeleteFile(LOCAL_FACEBOOK_DATA_FILE);
                Debug.Log(e.ToString());
            }
        }

        public Sprite GetPic(string playerId)
        {
            try
            {
                if (localDataService.FileExists(LOCAL_FACEBOOK_DATA_FILE))
                {
                    ILocalDataReader reader = localDataService.OpenReader(LOCAL_FACEBOOK_DATA_FILE);
                    string key = LOCAL_FACEBOOK_DATA_PIC + playerId;
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
    }
}

