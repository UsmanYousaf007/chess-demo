/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using strange.extensions.promise.api;
using UnityEngine;
using UnityEngine.U2D;

namespace TurboLabz.InstantFramework
{
    public class DownloadablesService : IDownloadablesService
    {
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IDownloadablesModel downloadablesModel { get; set; }

        private string downloadShortCode;
        private Action<BackendResult, AssetBundle> onDownloadContentCompleteCB;

        public void GetDownloadableContent(string shortCode, Action<BackendResult, AssetBundle> callbackFn)
        {
            // Note: GetDownloadableContentRequest is not in the Strange context, but it uses a function from IBackendService. So the backend service
            // function delegate is passed in to GetDownloadableContentRequest as a parameter.

            DownloadableItem dlItem = downloadablesModel.downloadableItems[shortCode];
            if (dlItem == null)
            {
                OnDownloadContentComplete(BackendResult.DOWNLOADABLE_CONTENT_GET_FAILED, null);
                return;
            }

            // Check for bundle already loaded and persistent
            if (dlItem.bundle != null)
            {
                callbackFn?.Invoke(BackendResult.SUCCESS, dlItem.bundle);
                return;
            }

            downloadShortCode = shortCode;
            onDownloadContentCompleteCB = callbackFn;

            if (downloadablesModel.IsUpdateAvailable(shortCode))
            {
                TLUtils.LogUtil.Log("GetDownloadableContent()==> Download from URL", "cyan");
                new GetDownloadableContentRequest().Send(backendService.GetDownloadableContentUrl, dlItem.shortCode, dlItem.lastModified).Then(OnDownloadContentComplete);
            }

            else
            {
                TLUtils.LogUtil.Log("GetDownloadableContent()==> Fetch from Cache", "cyan");
                new GetDownloadableContentRequest().Send(null, dlItem.shortCode, dlItem.lastModified).Then(OnDownloadContentComplete);
            }
        }

        public void OnDownloadContentComplete(BackendResult result, AssetBundle bundle)
        {
            if (result == BackendResult.SUCCESS)
            {
                downloadablesModel.downloadableItems[downloadShortCode].bundle = bundle;
                downloadablesModel.MarkUpdated(downloadShortCode);

                string[] all = bundle.GetAllAssetNames();
                TLUtils.LogUtil.Log("----> Bundle assets <---- " + all.Length, "yellow");
                for (int i = 0; i < all.Length; i++)
                {
                    TLUtils.LogUtil.Log("bundle asset: " + all[i], "yellow");
                }
            }

            onDownloadContentCompleteCB?.Invoke(result, bundle);
        }
    }
}
