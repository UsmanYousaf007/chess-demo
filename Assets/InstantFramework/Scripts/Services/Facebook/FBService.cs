/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using Facebook.Unity;
using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public class FBService : IFacebookService
    {
        public const string PLAYER_USER_ID_ALIAS = "me";

        private const string LOCAL_FACEBOOK_DATA_FILE = "localFacebookDataFile";
        private const string LOCAL_FACEBOOK_DATA_PIC = "localFacebookDataPic";

        // Services
        [Inject] public ILocalDataService localDataService { get; set; }

        public IPromise<FacebookResult> Init()
        {
            return new FBInitRequest().Send();
        }

        public IPromise<FacebookResult, string> Auth()
        {
            return new FBAuthRequest().Send();
        }

        public IPromise<FacebookResult, Sprite> GetSocialPic(string userId)
        {
            return new FBGetSocialPicRequest().Send(userId, CachePlayerPic);
        }

        public IPromise<FacebookResult, string> GetSocialName()
        {
            return new FBSocialNameRequest().Send();
        }

        public bool isLoggedIn()
        {
            return FB.IsLoggedIn;
        }

        public string GetPlayerUserIdAlias()
        {
            return PLAYER_USER_ID_ALIAS;
        }

        public Sprite GetCachedPlayerPic()
        {
            if (localDataService.FileExists(LOCAL_FACEBOOK_DATA_FILE))
            {
                ILocalDataReader reader = localDataService.OpenReader(LOCAL_FACEBOOK_DATA_FILE);
                Sprite pic = reader.Read<Sprite>(LOCAL_FACEBOOK_DATA_PIC);
                reader.Close();
                return pic;
            }

            return null;
        }

        private void CachePlayerPic(Sprite sprite)
        {
            ILocalDataWriter writer = localDataService.OpenWriter(LOCAL_FACEBOOK_DATA_FILE);
            writer.Write(LOCAL_FACEBOOK_DATA_PIC, sprite);
            writer.Close();
        }
    }
}
