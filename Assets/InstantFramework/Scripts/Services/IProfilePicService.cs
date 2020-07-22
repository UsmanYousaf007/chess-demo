/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public interface IProfilePicService
    {
        IPromise<BackendResult, Sprite, string> GetProfilePic(string facebookUserId, string playerId);

        IPromise<BackendResult> UploadProfilePic(string filename, byte[] stream, string mimeType);
    }
}
