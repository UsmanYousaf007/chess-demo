using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public interface IDownloadablesModel
    {
        Dictionary<string, DownloadableItem> downloadableItems { get; set; }

        void Init();
        bool IsUpdateAvailable(string shortCode);
        void MarkUpdated(string shortCode);
        void Get(string shortCode, Action<BackendResult, AssetBundle> callbackFn = null, ContentType? contentType = null);
        void PreloadFromCache(string shortCode, bool shouldLoad);
        AssetBundle GetBundleFromVersionCache(string shortCode);
    }
}
