/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using Facebook.Unity;
using strange.extensions.promise.api;
using System;
using System.Collections.Generic;
using strange.extensions.promise.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class ProfilePicService : IProfilePicService
    {

        [Inject] public IBackendService backendService { get; set; }

        public IPromise<BackendResult, Sprite, string> GetProfilePic(string playerId, string uploadedPicId)
        {
            return new GetProfilPicRequest().Send(backendService.GetDownloadUrl, playerId, uploadedPicId);
        }

        public IPromise<BackendResult> UploadProfilePic(string filename, byte[] stream, string mimeType)
        {
            return null;
        }
    }
}