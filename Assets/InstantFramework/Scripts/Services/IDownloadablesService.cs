/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.promise.api;
using System;

namespace TurboLabz.InstantFramework
{
    public interface IDownloadablesService
    {
        void GetDownloadableContent(string shortCode, Action<BackendResult, AssetBundle> onDownloadCompleteCB, ContentType? contentType);
    }
}
