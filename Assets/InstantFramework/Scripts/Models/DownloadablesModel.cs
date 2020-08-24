using System.Collections.Generic;
using System;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Linq;

namespace TurboLabz.InstantFramework
{
    public class DownloadableItem
    {
        public string url { get; set; }
        public string shortCode { get; set; }
        public long size { get; set; }
        public long lastModified { get; set; }
        public AssetBundle bundle { get; set; }
        public bool loadFromCache { get; set; }
    }

    public enum ContentType
    {
        Skins
    }

    public enum ContentDownloadStatus
    {
        Started,
        Completed,
        Failed,
        Cancelled
    }

    public class DownloadablesModel : IDownloadablesModel
    {
        // Services
        [Inject] public ILocalDataService localDataService { get; set; }
        [Inject] public IDownloadablesService downloadablesService { get; set; }
        // Listen to signals   
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        public Dictionary<string, DownloadableItem> downloadableItems { get; set; }

        private const string VERSION_CACHE_FILENAME = "downloadables_versioncache";
        private Dictionary<string, DownloadableItem> versionCache;

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            downloadableItems = new Dictionary<string, DownloadableItem>();
        }

        public void Init()
        {
            versionCache = new Dictionary<string, DownloadableItem>();
            LoadVersionCache();
            downloadableItems = versionCache;
            LoadBundlesFromCachePreLaunch();
        }

        private void LoadBundlesFromCachePreLaunch()
        {
            foreach (var item in versionCache.Where(i=>i.Value.loadFromCache))
            {
                var shortCode = item.Value.shortCode;
                Get(shortCode);
            }
        }

        public void Get(string shortCode, Action<BackendResult, AssetBundle> callbackFn = null, ContentType? contentType = null )
        {   
            TLUtils.LogUtil.Log("BundleRefs - requesting download " + shortCode, "cyan");
            try
            {
                downloadablesService.GetDownloadableContent(shortCode, callbackFn, contentType);
            }

            catch (Exception e)
            {
                TLUtils.LogUtil.Log("Error when getting downloadable content. " + e, "red");
            }
        }

        public bool IsUpdateAvailable(string shortCode)
        {
            return !versionCache.ContainsKey(shortCode) ? true : versionCache[shortCode].lastModified < downloadableItems[shortCode].lastModified;
        }

        public void LoadFromCache(string shortCode, bool shouldLoad)
        {
            if (downloadableItems.ContainsKey(shortCode))
            {
                downloadableItems[shortCode].loadFromCache = shouldLoad;
                MarkUpdated(shortCode);
            }
        }

        public void MarkUpdated(string shortCode)
        {
            if (!versionCache.ContainsKey(shortCode))
            {
                versionCache.Add(shortCode, downloadableItems[shortCode]);
            }

            else
            {
                versionCache[shortCode] = downloadableItems[shortCode];
            }
            SaveVersionCache();
        }

        public AssetBundle GetBundleFromVersionCache(string shortCode)
        {
            if (versionCache.ContainsKey(shortCode))
            {
                return versionCache[shortCode].bundle;
            }
            else
            {
                return null;
            }
        }

        private void LoadVersionCache()
        {
            if (!localDataService.FileExists(VERSION_CACHE_FILENAME))
            {
                return;
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(VERSION_CACHE_FILENAME);
                versionCache = ReadVersionDictionary(reader);
                reader.Close();

                LogVersionDictionary(versionCache);
            }
            catch (Exception e)
            {
                TLUtils.LogUtil.Log("Corrupt saved downloadables version cache! " + e, "red");
                localDataService.DeleteFile(VERSION_CACHE_FILENAME);
            }
        }

        private void SaveVersionCache()
        {
            try
            {
                ILocalDataWriter writer = localDataService.OpenWriter(VERSION_CACHE_FILENAME);
                WriteVersionDictionary(writer, versionCache);
                writer.Close();
            }
            catch (Exception e)
            {
                if (localDataService.FileExists(VERSION_CACHE_FILENAME))
                {
                    localDataService.DeleteFile(VERSION_CACHE_FILENAME);
                }

                TLUtils.LogUtil.Log("Critical error when saving downloadables version cache file. File deleted. " + e, "red");
            }
        }

        private void WriteVersionDictionary(ILocalDataWriter writer, Dictionary<string, DownloadableItem> dict)
        {
            writer.Write<int>("count", versionCache.Count);

            int i = 0;
            foreach (KeyValuePair<string, DownloadableItem> dlItem in dict)
            {
                string iStr = i.ToString();
                writer.Write<string>("key" + iStr, dlItem.Key);
                writer.Write<string>("shortCode" + iStr, dlItem.Value.shortCode);
                writer.Write<long>("size" + iStr, dlItem.Value.size);
                writer.Write<long>("lastModified" + iStr, dlItem.Value.lastModified);
                writer.Write<bool>("loadFromCache" + iStr, dlItem.Value.loadFromCache);
                //writer.Write<string>("url" + iStr, dlItem.Value.url);
                i++;
            }
        }

        private Dictionary<string, DownloadableItem> ReadVersionDictionary(ILocalDataReader reader)
        {
            Dictionary<string, DownloadableItem> dict = new Dictionary<string, DownloadableItem>();
            int count = reader.Read<int>("count");

            for (int i = 0; i < count; i++)
            {
                string iStr = i.ToString();
                DownloadableItem dlItem = new DownloadableItem();

                string key = reader.Read<string>("key" + iStr);
                dlItem.shortCode = reader.Read<string>("shortCode" + iStr);
                dlItem.size = reader.Read<long>("size" + iStr);
                dlItem.lastModified = reader.Read<long>("lastModified" + iStr);
                dlItem.loadFromCache = reader.Read<bool>("loadFromCache" + iStr);
                //dlItem.url = reader.Read<string>("url" + iStr);

                dict.Add(key, dlItem);
            }

            return dict;
        }

        private void LogVersionDictionary(Dictionary<string, DownloadableItem> dict)
        {
            TLUtils.LogUtil.Log("------>> Downloadables Version Cache Dictionary Begin <<------");

            foreach (KeyValuePair<string, DownloadableItem> dlItem in dict)
            {
                TLUtils.LogUtil.Log("------>> key: " + dlItem.Key);
                TLUtils.LogUtil.Log("------>> shortCode: " + dlItem.Value.shortCode);
                TLUtils.LogUtil.Log("------>> size: " + dlItem.Value.size.ToString());
                TLUtils.LogUtil.Log("------>> lastModified: " + dlItem.Value.lastModified.ToString());
                //TLUtils.LogUtil.Log("------>> url: " + dlItem.Value.url);
            }

            TLUtils.LogUtil.Log("------>> Downloadables Version Cache Dictionary End <<------");
        }
    }
}

