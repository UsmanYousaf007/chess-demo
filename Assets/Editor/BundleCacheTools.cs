using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantFramework;
using UnityEditor;
using UnityEngine;

public partial class ChessTools
{
#if UNITY_ANDROID
    private const string VERSION_CACHE_FILENAME = "downloadables_versioncache_andriod";
#elif UNITY_IOS
        private const string VERSION_CACHE_FILENAME = "downloadables_versioncache_ios";
#endif
    [MenuItem("TLTools/Asset Bundles/Clear Cache", false, 25)]
    public static void ClearSkinCache()
    {
        EasySaveService easySaveService = new EasySaveService();
        if (easySaveService.FileExists(VERSION_CACHE_FILENAME))
        {
            easySaveService.DeleteFile(VERSION_CACHE_FILENAME);
        }

        Caching.ClearCache();
    }

}

