/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using strange.extensions.promise.api;
using UnityEngine;
using UnityEngine.U2D;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class DownloadablesService : IDownloadablesService
    {
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IDownloadablesModel downloadablesModel { get; set; }
        [Inject] public DownloadableContentEventSignal dlcSignal { get; set; }
        private string shortCode;
        private Action<BackendResult, AssetBundle> onDownloadContentCompleteCB;

        //backup downloadable item
        private DownloadableItem _dlItem;

        public void GetDownloadableContent( string shortCode, Action<BackendResult, AssetBundle> callbackFn,
                                            ContentType? contentType)
        {
            // Note: GetDownloadableContentRequest is not in the Strange context, but it uses a function from IBackendService. So the backend service
            // function delegate and DownloadableContentSignal is passed in to GetDownloadableContentRequest.

            
            DownloadableItem dlItem = downloadablesModel?.downloadableItems?[shortCode];
            
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

            this.shortCode = shortCode;
            onDownloadContentCompleteCB = callbackFn;


            //backup the downloadable item in case it gets lost while the process is running to get the asset bundle
            _dlItem = new DownloadableItem();
            _dlItem.size = dlItem.size;
            _dlItem.downloadShortCode = dlItem.downloadShortCode;
            _dlItem.shortCode = dlItem.shortCode;
            _dlItem.lastModified = dlItem.lastModified;
            _dlItem.url = dlItem.url;
            _dlItem.bundle = dlItem.bundle;

            //if (downloadablesModel.IsUpdateAvailable(shortCode))
            //{
                new GetDownloadableContentRequest(dlcSignal, contentType).Send(backendService.GetDownloadableContentUrl,
                                                    dlItem.downloadShortCode,dlItem.shortCode, dlItem.lastModified)
                                                    .Then(OnDownloadContentComplete);
            //}

            //else
            //{
            //    new GetDownloadableContentRequest(dlcSignal, contentType).Send(null, dlItem.downloadShortCode,
            //                                        dlItem.shortCode, dlItem.lastModified)
            //                                        .Then(OnDownloadContentComplete);
            //}
        }

        public void OnDownloadContentComplete(BackendResult result, AssetBundle bundle)
        {
            TLUtils.LogUtil.Log("DownloadablesService::OnDownloadContentComplete() ==> result:" + result, "cyan");

            if (result == BackendResult.SUCCESS)
            {
                try
                {
                    if (downloadablesModel.downloadableItems[shortCode] == null)
                    {
                        downloadablesModel.downloadableItems[shortCode] = _dlItem;
                    }
                    downloadablesModel.downloadableItems[shortCode].bundle = bundle;
                    downloadablesModel.MarkUpdated(shortCode);
                }
                catch (Exception e)
                {
                    LogUtil.Log("Downloadable item not found " + e, "red");
                }

                TLUtils.LogUtil.Log("DownloadablesService::OnDownloadContentComplete() ==> bundle:" + bundle.name, "cyan");
            }
            
            onDownloadContentCompleteCB?.Invoke(result, bundle);
        }
    }
}
