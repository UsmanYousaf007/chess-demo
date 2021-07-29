using System;
using System.Collections;
using System.IO;
using System.Linq;
using HUF.Storage.Runtime.API;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.Utils.Runtime;
using UnityEngine;

namespace HUF.Storage.Runtime.Implementation.ActionHandlers
{
    public class AssetBundleLocalHandler : BaseActionHandler<AssetBundle>
    {
        const string ASSET_BUNDLE_LOADING_ERROR = "Asset bundle loading error. Asset bundle not existing or corrupted";

        public AssetBundleLocalHandler(string fileId, Action<ObjectResultContainer<AssetBundle>> completeHandler)
            : base(fileId, completeHandler)
        {
        }

        public AssetBundleLocalHandler(string fileId, Action<ObjectResultContainer<AssetBundle>> completeHandler, string filePath)
            : base(fileId, completeHandler)
        {
            FilePath = filePath;
        }

        public override void DownloadFile()
        {
            var assetBundle = TryGetAssetBundle(Path.GetFileName(FilePath));
            if (assetBundle == null)
            {
                CoroutineManager.StartCoroutine(
                    LoadFromFileAsync(StorageUtils.GetLocalFilePath(FilePath)));
            }
            else
            {
                SendHandlerSuccess(assetBundle);
            }
        }

        protected AssetBundle TryGetAssetBundle(string assetBundleName)
        {
            var assetBundles = AssetBundle.GetAllLoadedAssetBundles();
            return assetBundles.FirstOrDefault(assetBundle => assetBundle.name == assetBundleName);
        }

        IEnumerator LoadFromFileAsync(string localFilePath)
        {
            var assetBundleAsyncRequest = AssetBundle.LoadFromFileAsync(localFilePath);

            yield return assetBundleAsyncRequest;

            var assetBundle = assetBundleAsyncRequest.assetBundle;
            if (assetBundle == null)
            {
                SendHandlerFail(ASSET_BUNDLE_LOADING_ERROR);
                yield break;
            }
            SendHandlerSuccess(assetBundle);
        }
    }
}