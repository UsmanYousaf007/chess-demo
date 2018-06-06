/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-29 17:55:23 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public class FBService : IFacebookService
    {
        public IPromise<FacebookResult> Init()
        {
            return new FBInitRequest().Send();
        }

        public IPromise<FacebookResult, string> Auth()
        {
            return new FBAuthRequest().Send();
        }

        public IPromise<FacebookResult, Texture2D> GetProfilePicture(string userId)
        {
            return new FBGetProfilePictureRequest().Send(userId);
        }

        public IPromise<FacebookResult, string> GetUserName()
        {
            return new FBUserNameRequest().Send();
        }
    }
}
